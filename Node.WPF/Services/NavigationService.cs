using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Node.WPF.Services
{
    public class NavigationService : INotifyPropertyChanged
    {
        private readonly IServiceProvider _sp;

        private object? _currentViewModel;
        public object? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                if (Equals(_currentViewModel, value)) return;
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(IServiceProvider sp)
        {
            _sp = sp;
        }

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            CurrentViewModel = _sp.GetRequiredService<TViewModel>();
        }

        public void NavigateTo(object viewModel) => CurrentViewModel = viewModel;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}