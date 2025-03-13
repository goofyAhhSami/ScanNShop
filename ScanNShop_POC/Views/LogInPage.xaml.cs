using ScanNShop_POC.Database;
using ScanNShop_POC.Services;
using System;

namespace ScanNShop_POC.Views
{
    public partial class LogInPage : ContentPage
    {
        private readonly ApiService _apiService;

        public LogInPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Fehler", "Bitte Benutzername und Passwort eingeben.", "OK");
                return;
            }

            var token = await _apiService.Login(username, password);
            if (token != null)
            {
                // Hier den Token speichern und den Nutzer weiterleiten
                var dbService = new LocalDbService();
                Application.Current.MainPage = new AppShell(dbService);
            }
            else
            {
                await DisplayAlert("Fehler", "Falsche Anmeldeinformationen.", "OK");
            }
        }

        private void OnRegisterTapped(object sender, EventArgs e)
        {
            Overlay.IsVisible = true;
            RegisterPopup.IsVisible = true;

            // Animation für das Abdunkeln des Hintergrunds
            Overlay.FadeTo(0.5, 250);
        }

        private void OnPopupCloseClicked(object sender, EventArgs e)
        {
            Overlay.FadeTo(0, 250);
            RegisterPopup.IsVisible = false;
            Overlay.IsVisible = false;
        }

        private async void OnPopupRegisterClicked(object sender, EventArgs e)
        {
            string email = PopupEmailEntry.Text;
            string username = PopupUsernameEntry.Text;
            string password = PopupPasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Fehler", "Bitte fülle alle Felder aus.", "OK");
                return;
            }

            var success = await _apiService.Register(email, username, password);
            if (success)
            {
                await DisplayAlert("Erfolg", "Benutzer erfolgreich registriert!", "OK");
                OnPopupCloseClicked(sender, e);
            }
            else
            {
                await DisplayAlert("Fehler", "Registrierung fehlgeschlagen.", "OK");
            }
        }

        private void OnSuccessPopupCloseClicked(object sender, EventArgs e)
        {
            Overlay.FadeTo(0, 250);
            PopupSuccess.IsVisible = false;
            Overlay.IsVisible = false;
        }
    }
}
