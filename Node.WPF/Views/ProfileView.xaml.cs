using System.Windows.Controls;
using Node.WPF.ViewModels;

namespace Node.WPF.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel();
        }
    }
}
