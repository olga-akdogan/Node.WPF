using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Models;
using Node.WPF.Helpers;
using Node.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Node.WPF.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly Session _session;
        private readonly NavigationService _nav;

        public ObservableCollection<Profile> Profiles { get; } = new();

        private Profile? _selectedProfile;
        public Profile? SelectedProfile
        {
            get => _selectedProfile;
            set { _selectedProfile = value; OnPropertyChanged(); RaiseCommands(); }
        }

        private string _status = "";
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); RaiseCommands(); }
        }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand SoftDeleteCommand { get; }
        public RelayCommand RestoreCommand { get; }
        public RelayCommand BackCommand { get; }

        public AdminViewModel(
            IDbContextFactory<AppDbContext> dbFactory,
            Session session,
            NavigationService nav)
        {
            _dbFactory = dbFactory;
            _session = session;
            _nav = nav;

            RefreshCommand = new RelayCommand(() => _ = LoadAsync(), () => !IsBusy);
            SoftDeleteCommand = new RelayCommand(() => _ = SoftDeleteAsync(), () => !IsBusy && SelectedProfile != null && !SelectedProfile.IsDeleted);
            RestoreCommand = new RelayCommand(() => _ = RestoreAsync(), () => !IsBusy && SelectedProfile != null && SelectedProfile.IsDeleted);
            BackCommand = new RelayCommand(() => _nav.NavigateTo<HomeViewModel>(), () => !IsBusy);

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            if (!_session.IsAuthenticated || !_session.IsAdmin)
            {
                Status = "Not authorized.";
                return;
            }

            try
            {
                IsBusy = true;
                Status = "Loading profiles…";
                Profiles.Clear();

                await using var db = await _dbFactory.CreateDbContextAsync();

               
                var list = await db.Profiles
                    .IgnoreQueryFilters()
                    .Include(p => p.AppUser)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                foreach (var p in list)
                    Profiles.Add(p);

                Status = $"Loaded {Profiles.Count} profiles (including deleted).";
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SoftDeleteAsync()
        {
            if (SelectedProfile == null) return;

            try
            {
                IsBusy = true;
                Status = "Soft deleting…";

                await using var db = await _dbFactory.CreateDbContextAsync();

               
                var entity = await db.Profiles.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(p => p.Id == SelectedProfile.Id);

                if (entity == null)
                {
                    Status = "Profile not found.";
                    return;
                }

                db.Profiles.Remove(entity);
                await db.SaveChangesAsync();

                Status = "Soft deleted.";
                await LoadAsync();
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RestoreAsync()
        {
            if (SelectedProfile == null) return;

            try
            {
                IsBusy = true;
                Status = "Restoring…";

                await using var db = await _dbFactory.CreateDbContextAsync();

                var entity = await db.Profiles.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(p => p.Id == SelectedProfile.Id);

                if (entity == null)
                {
                    Status = "Profile not found.";
                    return;
                }

                entity.IsDeleted = false;
                entity.DeleteAt = null; 
                await db.SaveChangesAsync();

                Status = "Restored.";
                await LoadAsync();
            }
            catch (Exception ex)
            {
                Status = "Error: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void RaiseCommands()
        {
            RefreshCommand.RaiseCanExecuteChanged();
            SoftDeleteCommand.RaiseCanExecuteChanged();
            RestoreCommand.RaiseCanExecuteChanged();
            BackCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}