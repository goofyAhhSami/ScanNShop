using ScanNShop_POC.Database;
using ScanNShop_POC.Views;

namespace ScanNShop_POC
{
    public partial class MainPage : ContentPage
    {

        private readonly LocalDbService _dbService;
        private int _editListId;
        private string _fullText = "Willkommen bei ScanNShop !";
        private int _currentIndex = 0;
        private bool _isAnimating = false;

        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            StartTextAnimation();
            _dbService = dbService;
            Task.Run(UpdateListViewAsync); // Liste initialisieren

            MessagingCenter.Subscribe<ListEdit>(this, "ListDeleted", async (sender) =>
            {
                // Aktualisiere die ListView, wenn eine Liste gelöscht wird
                await UpdateListViewAsync();
            });

        }

        private async void createNewList(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.NewList(_dbService, _editListId), true);

            // Skalierung
            await CreateList.ScaleTo(1.1, 150, Easing.CubicInOut);
            await CreateList.ScaleTo(1, 150, Easing.CubicInOut);

        }

        private async void openKochPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AiPage());

        }


        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            var liste = (Liste)e.Item;
            var action = await DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

            /* switch (action)
             {
                  case "Edit":
                      _editListId = liste.listId;
                      nameEntryField.Text = liste.Name;


                      break;

                  case "Delete":
                      await _dbService.Delete(liste);
                      listView.ItemsSource = await _dbService.GetLists();
                      break; 
             }*/
        }


        public async Task UpdateListViewAsync()
        {
            listView.ItemsSource = await _dbService.GetLists();        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            // Sicherstellen, dass der Button das richtige BindingContext enthält
            if (sender is Button button && button.BindingContext is Liste liste)
            {
                // Die Methode mit dem Element aufrufen
                await Navigation.PushAsync(new ListEdit(_dbService, liste.listId), true);
            }
        }


     

        // Methode: Alle Listen löschen
        private async void DeleteAllLists(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Bestätigung", "Möchten Sie wirklich alle Listen löschen?", "Ja", "Abbrechen");
            if (confirm)
            {
                await _dbService.DeleteAllListsAsync(); // Alle Listen löschen
                await UpdateListViewAsync(); // ListView leeren
            }
        }

        private async void StartTextAnimation()
        {
            if (_isAnimating)
                return;

            _isAnimating = true;
            animatedLabel.Text = string.Empty;

            while (true)
            {
                if (_currentIndex < _fullText.Length)
                {
                    animatedLabel.Text += _fullText[_currentIndex];
                    _currentIndex++;
                }
                else
                {
                    _currentIndex = 0;
                    animatedLabel.Text = string.Empty;
                    await Task.Delay(1000); // Verzögerung zwischen den Wiederholungen
                }

                await Task.Delay(210); // Wartezeit zwischen den Buchstaben in Millisekunden
            }
        }




    }

}
