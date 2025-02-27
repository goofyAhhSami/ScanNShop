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
            await DisplayAlert("Einstellungen", "Hier k�nnten Einstellungen ge�ffnet werden.", "OK");
        }

        private async void Logout(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Abmelden", "M�chtest du dich wirklich abmelden?", "Ja", "Nein");
            if (confirm)
            {
                // Hier k�nnte man zur Login-Seite weiterleiten oder die App schlie�en
                await DisplayAlert("Abgemeldet", "Du bist jetzt abgemeldet.", "OK");
            }
        }
    }
}
