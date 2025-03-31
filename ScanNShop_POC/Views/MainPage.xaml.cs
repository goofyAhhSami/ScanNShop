using Newtonsoft.Json;
using ScanNShop_POC.Database;
using ScanNShop_POC.DTO;
using ScanNShop_POC.Services;
using ScanNShop_POC.Views;

namespace ScanNShop_POC
{
    public partial class MainPage : ContentPage
    {
        private readonly LocalDbService _dbService;
        private int _editListId;
        private string _fullText = "ScanNShop";
        private int _currentIndex = 0;
        private bool _isAnimating = false;

        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            UpdateListViewAsync(); // Stelle sicher, dass die Listen geladen werden

            MessagingCenter.Subscribe<ListEdit>(this, "ListDeleted", async (sender) =>
            {
                await UpdateListViewAsync();
            });

            MessagingCenter.Subscribe<ListsPage>(this, "ListDeletedFromListsPage", async (sender) =>
            {
                await UpdateListViewAsync();
            });

        }

        private async void createNewList(object sender, EventArgs e)
        {
            PopupContainer.IsVisible = true;

            await CreateList.ScaleTo(1.1, 150, Easing.CubicInOut);
            await CreateList.ScaleTo(1, 150, Easing.CubicInOut);
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            PopupContainer.IsVisible = false;
        }



        private async void saveButton_Clicked(object sender, EventArgs e)
        {
            string listName = nameEntryField.Text;
            if (string.IsNullOrEmpty(listName))
            {
                await DisplayAlert("Fehler", "Bitte geben Sie einen Namen für die Liste ein.", "OK");
                return;
            }

            try
            {
                var neueListe = new Liste { Name = listName };
                await _dbService.Create(neueListe);
                Console.WriteLine($"✅ Liste '{listName}' wurde lokal erstellt.");

                // 🌐 Wenn Internet verfügbar → hochladen
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var userRaw = Preferences.Get("User", null);
                    Console.WriteLine($"[DEBUG] User-JSON: {userRaw}");

                    if (!string.IsNullOrEmpty(userRaw) && userRaw.Trim().StartsWith("{"))
                    {
                        var userObj = JsonConvert.DeserializeObject<User>(userRaw);
                        var api = ApiService.Instance;

                        var newListIdFromServer = await api.InsertList(new CreateListDto
                        {
                            Name = listName,
                            UserId = userObj.UserId
                        });

                        Console.WriteLine("NewListId: " + newListIdFromServer);
                        if (newListIdFromServer.HasValue)
                        {
                            // ❌ Lokale Liste löschen (falsche SQLite-ID)
                            await _dbService.Delete(neueListe);

                            // ✅ Neue Liste mit korrekter Server-ListId einfügen
                            var syncedListe = new Liste
                            {
                                ListId = newListIdFromServer.Value,
                                Name = listName,
                                CreationDate = DateTime.UtcNow
                            };

                            await _dbService.Create(syncedListe);
                            Console.WriteLine($"✅ Liste mit Server-ListId gespeichert: {syncedListe.ListId}");

                            Console.WriteLine($"✅ Server-ListId gespeichert: {newListIdFromServer.Value}");
                        }
                    }


                }

                await UpdateListViewAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Erstellen der Liste: {ex.Message}");
                await DisplayAlert("Fehler", $"Die Liste konnte nicht gespeichert werden: {ex.Message}", "OK");
            }

            PopupContainer.IsVisible = false;
        }



        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var liste = (Liste)e.Item;
            var action = await DisplayActionSheet("Aktion auswählen", "Abbrechen", null, "Bearbeiten", "Löschen");

            switch (action)
            {
                case "Bearbeiten":
                    _editListId = liste.ListId;
                    nameEntryField.Text = liste.Name;
                    break;

                case "Löschen":
                    await _dbService.Delete(liste);
                    await UpdateListViewAsync();
                    break;
            }
        }

        public async Task UpdateListViewAsync()
        {
            listView.ItemsSource = await _dbService.GetLists();
        }

       
        private async void OnButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Liste liste)
            {
                await Shell.Current.GoToAsync($"{nameof(ListEdit)}?listId={liste.ListId}");
            }
        }

        private async void DeleteAllLists(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Bestätigung", "Möchten Sie wirklich alle Listen löschen?", "Ja", "Abbrechen");
            if (confirm)
            {
                await _dbService.DeleteAllListsAsync();
                await UpdateListViewAsync();
            }
        }

        
    }
}
