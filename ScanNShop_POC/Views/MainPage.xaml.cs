using ScanNShop_POC.Database;
using ScanNShop_POC.MVVM.View;

namespace ScanNShop_POC
{
    public partial class MainPage : ContentPage
    {

        private readonly LocalDbService _dbService;
        private int _editListId;

        public MainPage(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            Task.Run(async () => listView.ItemsSource = await _dbService.GetLists());
        }

        private void createNewList(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NewList(_dbService, _editListId));
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
             listView.ItemsSource = await _dbService.GetLists();
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            // Sicherstellen, dass der Button das richtige BindingContext enthält
            if (sender is Button button && button.BindingContext is Liste liste)
            {
                // Die Methode mit dem Element aufrufen
                await Navigation.PushAsync(new ListEdit(_dbService, liste.listId));
            }
        }



    }

}
