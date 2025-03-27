using ScanNShop_POC.Database;
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

                Console.WriteLine($"✅ Liste '{listName}' wurde erfolgreich erstellt.");
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
                    _editListId = liste.listId;
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

        // ✅ KORREKTE NAVIGATION ZUR LISTEDIT SEITE
        private async void OnButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Liste liste)
            {
                await Shell.Current.GoToAsync($"{nameof(ListEdit)}?listId={liste.listId}");
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
