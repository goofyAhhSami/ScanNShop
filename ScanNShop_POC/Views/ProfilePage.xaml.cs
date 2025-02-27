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
                // Hier könnte man zur Login-Seite weiterleiten oder die App schließen
                await DisplayAlert("Abgemeldet", "Du bist jetzt abgemeldet.", "OK");
            }
        }
    }
}
