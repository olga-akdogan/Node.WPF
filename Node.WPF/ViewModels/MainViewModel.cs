using Node.WPF.Helpers;
using Node.WPF.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowHomeCommand { get; }
        public ICommand ShowProfileCommand { get; }
        public ICommand ShowMessagesCommand { get; }

        public MainViewModel()
        {
            ShowHomeCommand = new RelayCommand(() => CurrentView = new HomeView());
            ShowProfileCommand = new RelayCommand(() => CurrentView = new ProfileView());
            ShowMessagesCommand = new RelayCommand(() => CurrentView = new MessagesView());

            CurrentView = new RegisterView();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
