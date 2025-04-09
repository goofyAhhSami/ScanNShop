using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
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
            _apiService = ApiService.Instance;
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

            ShowLoginOverlay();

            try
            {
                var token = await _apiService.Login(username, password);
                if (token != null)
                {
                    Preferences.Set("auth_token", token);
                    Preferences.Set("IsGuest", false);
                    _apiService.SetAuthToken(token);

                    var dbService = LocalDbService.Instance;
                    await dbService.DeleteAllListsAsync();
                    await dbService.DeleteAllProductsAsync();

                    var user = await _apiService.GetUserByUsername(username);
                    if (user == null)
                    {
                        await DisplayAlert("Fehler", "Benutzer konnte nicht geladen werden.", "OK");
                        HideLoginOverlay();
                        return;
                    }

                    var userJson = JsonConvert.SerializeObject(user);
                    Preferences.Set("User", userJson);

                    var listen = await _apiService.GetListsByUserId(user.UserId);

                    foreach (var liste in listen)
                    {
                        await dbService.Create(new Liste
                        {
                            ListId = liste.ListId,
                            Name = liste.Name,
                            CreationDate = DateTime.UtcNow
                        });
                    }

                    var listIds = listen.Select(l => l.ListId).ToList();
                    var produkte = await _apiService.GetProductsByListIds(listIds);
                    foreach (var produkt in produkte)
                    {
                        await dbService.CreateProduct(new Product
                        {
                            ProductId = produkt.ProductId,
                            ListId = produkt.ListId,
                            Name = produkt.Name,
                            Quantity = produkt.Quantity,
                            IsChecked = produkt.IsChecked
                        });
                    }

                    Application.Current.MainPage = new AppShell(dbService);
                }
                else
                {
                    await DisplayAlert("Fehler", "Falsche Anmeldeinformationen.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login Exception: {ex.Message}");
                await DisplayAlert("Fehler", "Beim Login ist ein Fehler aufgetreten.", "OK");
            }
            finally
            {
                HideLoginOverlay();
            }
        }

        private void ShowLoginOverlay()
        {
            LoginLoadingOverlay.Opacity = 0;
            LoginLoadingOverlay.IsVisible = true;
            LoginLoadingOverlay.FadeTo(1, 250);
        }

        private async void HideLoginOverlay()
        {
            await LoginLoadingOverlay.FadeTo(0, 250);
            LoginLoadingOverlay.IsVisible = false;
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

        private void OnGuestTapped(object sender, EventArgs e)
        {
            var dbService = LocalDbService.Instance;
            _= dbService.DeleteAllListsAsync();
            _ = dbService.DeleteAllProductsAsync();
         
            // Speichern, dass der Benutzer ein Gast ist
            Preferences.Set("IsGuest", true);
            Preferences.Clear("auth_token");
            Preferences.Set("User", "{}");
            Preferences.Clear("User");

            Application.Current.MainPage = new AppShell(dbService);
        }


        private void OnSuccessPopupCloseClicked(object sender, EventArgs e)
        {
            Overlay.FadeTo(0, 250);
            PopupSuccess.IsVisible = false;
            Overlay.IsVisible = false;
        }
    }
}
