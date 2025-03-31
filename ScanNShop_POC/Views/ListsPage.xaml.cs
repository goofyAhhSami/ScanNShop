using ScanNShop_POC.Database;
using ScanNShop_POC.Services;

namespace ScanNShop_POC.Views;

public partial class ListsPage : ContentPage
{
    private LocalDbService _dbService;
    private bool _isPageActive = false;


    public ListsPage()
    {
        InitializeComponent();
        _dbService = LocalDbService.Instance;  
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_dbService == null)
        {
            Console.WriteLine("_dbService ist NULL in ListsPage!");
            return;
        }

     
        await UpdateListViewAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isPageActive = false;
    }

    public async Task UpdateListViewAsync()
    {
        if (_dbService == null)
        {
            Console.WriteLine("FEHLER: _dbService ist NULL in UpdateListViewAsync!");
            return;
        }

        var lists = await _dbService.GetLists();
        Console.WriteLine($"Anzahl der Listen geladen: {lists.Count}");

        // Wichtig: UI-Elemente müssen im Main-Thread aktualisiert werden
        MainThread.BeginInvokeOnMainThread(() =>
        {
            listView.ItemsSource = lists;
        });
    }



    private async void OnListTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Liste selectedList)
        {
            await Shell.Current.GoToAsync($"{nameof(ListEdit)}?listId={selectedList.ListId}");

        }
    }

    private async void DeleteList(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Liste list)
        {
            bool confirm = await DisplayAlert("Löschen", $"Möchtest du die Liste '{list.Name}' wirklich löschen?", "Ja", "Nein");
            if (confirm)
            {
                // 🛠️ Selektion aufheben bevor Daten geändert werden
                listView.SelectedItem = null;

                // 🌐 Wenn online → auch am Server löschen
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var api = ApiService.Instance;
                    bool deleted = await api.DeleteListFromServer(list.ListId);
                    if (deleted)
                    {
                        Console.WriteLine($"✅ Liste {list.ListId} vom Server gelöscht.");
                    }
                }

                // 🔨 Lokal löschen
                await _dbService.Delete(list);

                MessagingCenter.Send(this, "ListDeletedFromListsPage");
                // await UpdateListViewAsync();
                await Shell.Current.GoToAsync("ListsPage");
            }
        }
    }



}
