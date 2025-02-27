using ScanNShop_POC.Database;

namespace ScanNShop_POC.Views;

public partial class LogInPage : ContentPage
{
    public LogInPage()
    {
        InitializeComponent();
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

        if (username == "sami" && password == "1234")
        {

            var dbService = new LocalDbService();

            Application.Current.MainPage = new AppShell(dbService);

            // ODER: Setze die MainPage als neue Hauptseite (ohne TabBar)
            // Application.Current.MainPage = new MainPage(dbService);
        }
        else
        {
            await DisplayAlert("Fehler", "Falsche Anmeldeinformationen.", "OK");
        }
    }

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        // Navigiere zur Registrierungsseite
        //await Navigation.PushAsync(new RegisterPage());
    }
}