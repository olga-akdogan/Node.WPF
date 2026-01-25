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

        public string DisplayName => _session.DisplayName ?? "";
        public string Email => _session.Email ?? "";

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy == value) return; _isBusy = value; OnPropertyChanged(); }
        }

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            set { if (_statusText == value) return; _statusText = value; OnPropertyChanged(); }
        }

        private Profile? _profile;
        public Profile? Profile
        {
            get => _profile;
            private set { _profile = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasProfile)); OnPropertyChanged(nameof(HasChart)); }
        }

        public bool HasProfile => Profile != null;
        public bool HasChart => Profile?.NatalChart != null && !string.IsNullOrWhiteSpace(Profile.NatalChart.ChartJson);

        public ICommand RefreshCommand { get; }
        public ICommand EditBirthDataCommand { get; }

        public ProfileViewModel(IDbContextFactory<AppDbContext> dbFactory, Session session, NavigationService nav)
        {
            _dbFactory = dbFactory;
            _session = session;
            _nav = nav;

            RefreshCommand = new RelayCommand(() => _ = LoadAsync(), () => !IsBusy);
            EditBirthDataCommand = new RelayCommand(() => _nav.NavigateTo<BirthDataViewModel>());

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;

            if (!_session.IsAuthenticated || string.IsNullOrWhiteSpace(_session.UserId))
            {
                StatusText = "Not logged in.";
                Profile = null;
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
                    : "Loaded ✅";
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
            finally
            {
                IsBusy = false;
                if (RefreshCommand is RelayCommand rc) rc.RaiseCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}