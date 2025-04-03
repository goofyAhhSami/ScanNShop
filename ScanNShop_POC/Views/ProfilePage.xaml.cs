using Newtonsoft.Json;
using ScanNShop_POC.Database;

namespace ScanNShop_POC.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        private async void OpenSettings(object sender, EventArgs e)
        {
            await DisplayAlert("Einstellungen", "Hier könnten Einstellungen geöffnet werden.", "OK");
        }

        private async void Logout(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Abmelden", "Möchtest du dich wirklich abmelden?", "Ja", "Nein");
            if (confirm)
            {
                Preferences.Remove("auth_token"); // Löscht den gespeicherten Token

                // Navigation zurück zur LoginPage und Stack leeren
                Application.Current.MainPage = new NavigationPage(new LogInPage());
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            profileFrame.Opacity = 0;
            await profileFrame.FadeTo(1, 400, Easing.CubicInOut);

            var userJson = Preferences.Get("User", null);
            if (!string.IsNullOrEmpty(userJson))
            {
                try
                {
                    var user = JsonConvert.DeserializeObject<User>(userJson);

                    usernameLabel.Text = user?.Username ?? "Unbekannt";
                    emailLabel.Text = $"E-Mail: {user?.Email ?? "-"}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ApFehler beim Laden des Benutzers: {ex.Message}");
                }
            }
        }


    }
}
