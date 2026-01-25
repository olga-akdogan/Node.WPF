using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.WPF.Helpers;
using Node.WPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private readonly Session _session;
        private readonly AuthService _auth;
        private readonly NavigationService _nav;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public string DisplayName => _session.DisplayName ?? "";
        public string Email => _session.Email ?? "";
        public bool IsAuthenticated => _session.IsAuthenticated;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();

                if (RefreshCommand is RelayCommand rc) rc.RaiseCanExecuteChanged();
            }
        }

        private string _statusText = "";
        public string StatusText
        {
            get => _statusText;
            set { if (_statusText == value) return; _statusText = value; OnPropertyChanged(); }
        }

        private bool _hasProfile;
        public bool HasProfile
        {
            get => _hasProfile;
            private set { if (_hasProfile == value) return; _hasProfile = value; OnPropertyChanged(); OnPropertyChanged(nameof(NeedsBirthData)); }
        }

        private bool _hasChart;
        public bool HasChart
        {
            get => _hasChart;
            private set { if (_hasChart == value) return; _hasChart = value; OnPropertyChanged(); OnPropertyChanged(nameof(NeedsBirthData)); }
        }

        public bool NeedsBirthData => !HasProfile || !HasChart;

        public ICommand RefreshCommand { get; }
        public ICommand GoProfileCommand { get; }
        public ICommand GoBirthDataCommand { get; }
        public ICommand LogoutCommand { get; }

        public HomeViewModel(Session session, AuthService auth, NavigationService nav, IDbContextFactory<AppDbContext> dbFactory)
        {
            _session = session;
            _auth = auth;
            _nav = nav;
            _dbFactory = dbFactory;

            RefreshCommand = new RelayCommand(() => _ = LoadAsync(), () => !IsBusy);
            GoProfileCommand = new RelayCommand(() => _nav.NavigateTo<ProfileViewModel>());
            GoBirthDataCommand = new RelayCommand(() => _nav.NavigateTo<BirthDataViewModel>());
            LogoutCommand = new RelayCommand(Logout);

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;

            if (!IsAuthenticated || string.IsNullOrWhiteSpace(_session.UserId))
            {
                StatusText = "Not logged in.";
                HasProfile = false;
                HasChart = false;
                return;
            }

            IsBusy = true;
            StatusText = "Loading…";

            try
            {
                await using var db = await _dbFactory.CreateDbContextAsync();

                var profile = await db.Profiles
                    .AsNoTracking()
                    .Include(p => p.NatalChart)
                    .FirstOrDefaultAsync(p => p.AppUserId == _session.UserId);

                HasProfile = profile != null;
                HasChart = profile?.NatalChart != null && !string.IsNullOrWhiteSpace(profile.NatalChart.ChartJson);

                if (!HasProfile)
                    StatusText = "Welcome! Complete your birth data to generate your chart.";
                else if (!HasChart)
                    StatusText = "Profile found, but natal chart is missing. Add/confirm birth data.";
                else
                    StatusText = "All set ✅";
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
                HasProfile = false;
                HasChart = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Logout()
        {
            _auth.Logout();
            _nav.NavigateTo<LoginViewModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}