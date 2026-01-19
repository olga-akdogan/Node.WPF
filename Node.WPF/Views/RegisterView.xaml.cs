using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.WPF.Services;


namespace Node.WPF.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private async void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            try
            {
                var email = EmailBox.Text;
                var password = PwdBox.Password;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ErrorText.Text = "Email and password are required.";
                    return;
                }

                await using var db = AppDbContextFactory.Create();

                var auth = new AuthService(db);
                var user = await auth.RegisterAsync(email, PasswordHasher.Hash(password));

                Session.CurrentUserLocalId = user.Id;

                var mainVm = (ViewModels.MainViewModel)Application.Current.MainWindow.DataContext;
                mainVm.CurrentView = new BirthDataView();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }
    }
}   
