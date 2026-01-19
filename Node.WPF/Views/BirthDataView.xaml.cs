using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.WPF.Services;

namespace Node.WPF.Views
{
    public partial class BirthDataView : UserControl
    {
        public BirthDataView()
        {
            InitializeComponent();
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            ErrorText.Text = "";

            if (Session.CurrentUserLocalId is null)
            {
                ErrorText.Text = "No user session found. Please register first.";
                return;
            }

            await using var db = AppDbContextFactory.Create();

            var user = await db.AppUsersLocal
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == Session.CurrentUserLocalId.Value);

            if (user?.Profile is null)
            {
                ErrorText.Text = "Profile not found for this user.";
                return;
            }

            BirthPlaceBox.Text = user.Profile.BirthPlace ?? "";
            BirthLatBox.Text = user.Profile.BirthLatitude.ToString(CultureInfo.InvariantCulture);
            BirthLonBox.Text = user.Profile.BirthLongitude.ToString(CultureInfo.InvariantCulture);

            BirthDatePicker.SelectedDate = user.Profile.BirthDateTimeUtc.Date;
            BirthTimeBox.Text = user.Profile.BirthDateTimeUtc.ToString("HH:mm");
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            try
            {
                if (Session.CurrentUserLocalId is null)
                {
                    ErrorText.Text = "No user session found. Please register first.";
                    return;
                }
                
                if (BirthDatePicker.SelectedDate is null)
                {
                    ErrorText.Text = "Please select a birth date.";
                    return;
                }

                if (!TimeSpan.TryParseExact(BirthTimeBox.Text.Trim(), @"hh\:mm", CultureInfo.InvariantCulture, out var time))
                {
                    ErrorText.Text = "Birth time must be in HH:mm format (example: 08:30).";
                    return;
                }

                if (!double.TryParse(BirthLatBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                {
                    ErrorText.Text = "Latitude must be a number (use dot like 51.2).";
                    return;
                }

                if (!double.TryParse(BirthLonBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                {
                    ErrorText.Text = "Longitude must be a number (use dot like 4.4).";
                    return;
                }

                var birthPlace = BirthPlaceBox.Text.Trim();

                var date = BirthDatePicker.SelectedDate.Value.Date;
                var birthUtc = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0, DateTimeKind.Utc);

                await using var db = AppDbContextFactory.Create();

                var user = await db.AppUsersLocal
                    .Include(u => u.Profile)
                    .FirstOrDefaultAsync(u => u.Id == Session.CurrentUserLocalId.Value);

                if (user?.Profile is null)
                {
                    ErrorText.Text = "Profile not found for this user.";
                    return;
                }

                user.Profile.BirthDateTimeUtc = birthUtc;
                user.Profile.BirthPlace = birthPlace;
                user.Profile.BirthLatitude = lat;
                user.Profile.BirthLongitude = lon;

                await db.SaveChangesAsync();

                var mainVm = (ViewModels.MainViewModel)Application.Current.MainWindow.DataContext;
                mainVm.CurrentView = new ProfileView();
            }
            catch (Exception ex)
            {
                ErrorText.Text = ex.Message;
            }
        }
    }
}
