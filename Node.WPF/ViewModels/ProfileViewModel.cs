using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Models;
using Node.WPF.Helpers;
using Node.WPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly Session _session;
        private readonly NavigationService _nav;
        private readonly LoveProfileService _loveProfileService;

        public string DisplayName => _session.DisplayName ?? "";
        public string Email => _session.Email ?? "";

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                RaiseCommands();
            }
        }

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            private set
            {
                if (_statusText == value) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        private Profile? _profile;
        public Profile? Profile
        {
            get => _profile;
            private set
            {
                _profile = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasProfile));
                OnPropertyChanged(nameof(HasChart));
                GenerateLoveProfileCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasProfile => Profile != null;
        public bool HasChart => Profile?.NatalChart != null && !string.IsNullOrWhiteSpace(Profile.NatalChart.ChartJson);

        public ICommand RefreshCommand { get; }
        public ICommand EditBirthDataCommand { get; }

        // --- AI Love Profile ---
        private string _loveProfileText = "";
        public string LoveProfileText
        {
            get => _loveProfileText;
            set
            {
                if (_loveProfileText == value) return;
                _loveProfileText = value;
                OnPropertyChanged();
            }
        }

        private bool _isGeneratingLoveProfile;
        public bool IsGeneratingLoveProfile
        {
            get => _isGeneratingLoveProfile;
            set
            {
                if (_isGeneratingLoveProfile == value) return;
                _isGeneratingLoveProfile = value;
                OnPropertyChanged();
                GenerateLoveProfileCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand GenerateLoveProfileCommand { get; }

        public ProfileViewModel(
            IDbContextFactory<AppDbContext> dbFactory,
            Session session,
            NavigationService nav,
            LoveProfileService loveProfileService)
        {
            _dbFactory = dbFactory;
            _session = session;
            _nav = nav;
            _loveProfileService = loveProfileService;

            RefreshCommand = new RelayCommand(() => _ = LoadAsync(), () => !IsBusy);
            EditBirthDataCommand = new RelayCommand(() => _nav.NavigateTo<BirthDataViewModel>(), () => !IsBusy);

            GenerateLoveProfileCommand = new RelayCommand(
                () => _ = GenerateLoveProfileAsync(),
                () => CanGenerateLoveProfile());

            _ = LoadAsync();
        }

        private bool CanGenerateLoveProfile()
        {
            if (IsBusy) return false;
            if (IsGeneratingLoveProfile) return false;
            if (Profile == null) return false;

            if (Profile.BirthDateTimeUtc == default) return false;
            if (string.IsNullOrWhiteSpace(Profile.BirthPlace)) return false;

            // Your model uses double (non-nullable), so validate ranges instead of HasValue.
            if (Profile.BirthLatitude < -90 || Profile.BirthLatitude > 90) return false;
            if (Profile.BirthLongitude < -180 || Profile.BirthLongitude > 180) return false;

            return true;
        }

        private async Task GenerateLoveProfileAsync()
        {
            try
            {
                IsGeneratingLoveProfile = true;
                LoveProfileText = "Generating…";

                if (Profile == null)
                {
                    LoveProfileText = "No profile loaded.";
                    return;
                }

                var text = await _loveProfileService.GenerateAsync(Profile);
                LoveProfileText = text;
            }
            catch (Exception ex)
            {
                LoveProfileText = $"Error: {ex.Message}";
            }
            finally
            {
                IsGeneratingLoveProfile = false;
            }
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;

            if (!_session.IsAuthenticated || string.IsNullOrWhiteSpace(_session.UserId))
            {
                Profile = null;
                StatusText = "Not logged in.";
                return;
            }

            IsBusy = true;
            StatusText = "Loading profile…";

            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();

                Profile = await db.Profiles
                    .AsNoTracking()
                    .Include(p => p.BirthLocation)
                    .Include(p => p.NatalChart)
                    .FirstOrDefaultAsync(p => p.AppUserId == _session.UserId);

                StatusText = Profile == null
                    ? "No profile yet. Complete your birth data."
                    : "Loaded.";
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

        private void RaiseCommands()
        {
            if (RefreshCommand is RelayCommand rc) rc.RaiseCanExecuteChanged();
            if (EditBirthDataCommand is RelayCommand ec) ec.RaiseCanExecuteChanged();
            GenerateLoveProfileCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}