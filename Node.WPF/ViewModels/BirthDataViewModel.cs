using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Models;
using Node.WPF.Helpers;
using Node.WPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class BirthDataViewModel : INotifyPropertyChanged
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly Session _session;
        private readonly NavigationService _nav;
        private readonly ChartService _chartService;
        private readonly GoogleGeocodingService _geo;

        public BirthDataViewModel(
            IDbContextFactory<AppDbContext> dbFactory,
            Session session,
            NavigationService nav,
            ChartService chartService,
            GoogleGeocodingService geo)
        {
            _dbFactory = dbFactory;
            _session = session;
            _nav = nav;
            _chartService = chartService;
            _geo = geo;

            SaveCommand = new RelayCommand(() => _ = SaveAsync(), () => !IsBusy);
            FindLocationCommand = new RelayCommand(() => _ = FindLocationAsync(), () => !IsBusy);
        }

        private DateTime _birthDate = DateTime.UtcNow.Date;
        public DateTime BirthDate
        {
            get => _birthDate;
            set { if (_birthDate == value) return; _birthDate = value; OnPropertyChanged(); }
        }

        private string _birthTime = "20:05";
        public string BirthTime
        {
            get => _birthTime;
            set { if (_birthTime == value) return; _birthTime = value; OnPropertyChanged(); }
        }

        private string _birthPlace = "";
        public string BirthPlace
        {
            get => _birthPlace;
            set { if (_birthPlace == value) return; _birthPlace = value; OnPropertyChanged(); }
        }

        private double _birthLatitude;
        public double BirthLatitude
        {
            get => _birthLatitude;
            set { if (_birthLatitude.Equals(value)) return; _birthLatitude = value; OnPropertyChanged(); }
        }

        private double _birthLongitude;
        public double BirthLongitude
        {
            get => _birthLongitude;
            set { if (_birthLongitude.Equals(value)) return; _birthLongitude = value; OnPropertyChanged(); }
        }

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            set { if (_statusText == value) return; _statusText = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();

                if (SaveCommand is RelayCommand sc) sc.RaiseCanExecuteChanged();
                if (FindLocationCommand is RelayCommand fc) fc.RaiseCanExecuteChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand FindLocationCommand { get; }

        private DateTime GetBirthDateTimeUtc()
        {
            if (!TimeSpan.TryParse(BirthTime, out var time))
                throw new Exception("Invalid time format. Use HH:mm (e.g., 20:05).");

            var utc = BirthDate.Date + time;
            return DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        }

        private async Task FindLocationAsync()
        {
            if (IsBusy) return;

            StatusText = "";

            if (string.IsNullOrWhiteSpace(BirthPlace))
            {
                StatusText = "Type a birth place first (e.g., Brussels, Belgium).";
                return;
            }

            IsBusy = true;
            try
            {
                var (lat, lng) = await _geo.GeocodeAsync(BirthPlace.Trim());
                BirthLatitude = lat;
                BirthLongitude = lng;
                StatusText = "Location found ✅";
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;


            StatusText = "";


            if (!_session.IsAuthenticated || string.IsNullOrWhiteSpace(_session.UserId))
            {
                StatusText = "You must be logged in first.";
                return;
            }


            if (string.IsNullOrWhiteSpace(BirthPlace))
            {
                StatusText = "Birth place is required.";
                return;
            }


            IsBusy = true;


            try
            {
              
                StatusText = "Resolving location…";
                var (lat, lng) = await _geo.GeocodeAsync(BirthPlace.Trim());
                BirthLatitude = lat;
                BirthLongitude = lng;


              
                StatusText = "Calculating chart…";
                var (positions, aspects) = await _chartService.CalculateNatalChartAsync(
                GetBirthDateTimeUtc(),
                BirthLatitude,
                BirthLongitude);


                var chartJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    generatedAtUtc = DateTime.UtcNow,
                    birthDateTimeUtc = GetBirthDateTimeUtc(),
                    latitude = BirthLatitude,
                    longitude = BirthLongitude,
                    positions = positions.RootElement,
                    aspects = aspects.RootElement
                });


               
                await using var db = await _dbFactory.CreateDbContextAsync();
                var userId = _session.UserId!;


                var profile = await db.Profiles
                .IgnoreQueryFilters() // <-- IMPORTANT: avoids IsDeleted filter breaking updates
                .Include(p => p.BirthLocation)
                .Include(p => p.NatalChart)
                .FirstOrDefaultAsync(p => p.AppUserId == userId);


                if (profile == null)
                {
                    profile = new Profile { AppUserId = userId };
                    db.Profiles.Add(profile);
                }


               
                profile.IsDeleted = false;
                profile.DeleteAt = null;


                profile.BirthDateTimeUtc = GetBirthDateTimeUtc();
                profile.BirthPlace = BirthPlace.Trim();
                profile.BirthLatitude = BirthLatitude;
                profile.BirthLongitude = BirthLongitude;


                profile.BirthLocation ??= new BirthLocation();
                profile.BirthLocation.PlaceName = profile.BirthPlace;
                profile.BirthLocation.Latitude = profile.BirthLatitude;
                profile.BirthLocation.Longitude = profile.BirthLongitude;


                profile.NatalChart ??= new NatalChart();
                profile.NatalChart.ChartJson = chartJson;
                profile.NatalChart.CreatedAtUtc = DateTime.UtcNow;


                await db.SaveChangesAsync();


                StatusText = "Saved";
                _nav.NavigateTo<HomeViewModel>();
            }
            catch (DbUpdateConcurrencyException)
            {
                StatusText = "Database mismatch while saving. Please restart the app and try again.";
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static string BuildChartJson(Profile profile, object? chartResult)
        {
            
            if (chartResult is string s && !string.IsNullOrWhiteSpace(s))
                return s;

            
            if (chartResult is ValueTuple<JsonDocument, JsonDocument> tuple)
            {
                var positionsDoc = tuple.Item1;
                var aspectsDoc = tuple.Item2;

                return JsonSerializer.Serialize(new
                {
                    generatedAtUtc = DateTime.UtcNow,
                    birthDateTimeUtc = profile.BirthDateTimeUtc,
                    latitude = profile.BirthLatitude,
                    longitude = profile.BirthLongitude,
                    positions = positionsDoc.RootElement,
                    aspects = aspectsDoc.RootElement
                });
            }

           
            if (chartResult is JsonDocument doc)
                return doc.RootElement.GetRawText();

           
            return JsonSerializer.Serialize(new
            {
                generatedAtUtc = DateTime.UtcNow,
                birthDateTimeUtc = profile.BirthDateTimeUtc,
                latitude = profile.BirthLatitude,
                longitude = profile.BirthLongitude,
                result = chartResult
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}