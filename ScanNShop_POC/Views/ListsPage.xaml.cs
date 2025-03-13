using ScanNShop_POC.Database;

namespace ScanNShop_POC.Views;

public partial class ListsPage : ContentPage
{
    private LocalDbService _dbService;
    private bool _isPageActive = false;


    public ListsPage()
    {
        InitializeComponent();
        _dbService = new LocalDbService(); 
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_dbService == null)
        {
            Console.WriteLine("_dbService ist NULL in ListsPage!");
            return;
        }

        await _dbService.DebugDatabase();
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

        // Wichtig: UI-Elemente m�ssen im Main-Thread aktualisiert werden
        MainThread.BeginInvokeOnMainThread(() =>
        {
            listView.ItemsSource = lists;
        });
    }



    private async void OnListTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is Liste selectedList)
        {
            await Shell.Current.GoToAsync($"{nameof(ListEdit)}?listId={selectedList.listId}");

        }
    }

    private async void DeleteList(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Liste list)
        {
            bool confirm = await DisplayAlert("L�schen", $"M�chtest du die Liste '{list.Name}' wirklich l�schen?", "Ja", "Nein");
            if (confirm)
            {
                await _dbService.Delete(list);
                await UpdateListViewAsync();
            }
        }
    }

    public void SetDbService(LocalDbService dbService)
    {
        _dbService = dbService;
    }

}
