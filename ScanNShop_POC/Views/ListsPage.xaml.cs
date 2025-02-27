using ScanNShop_POC.Database;

namespace ScanNShop_POC.Views;

public partial class ListsPage : ContentPage
{
    private LocalDbService _dbService;
    private bool _isPageActive = false;


    public ListsPage(LocalDbService dbService)
    {
        InitializeComponent();
        SetDbService(dbService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _dbService.DebugDatabase();
        _isPageActive = true;
        await UpdateListViewAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isPageActive = false;
    }

    private async Task UpdateListViewAsync()
    {
        var lists = await _dbService.GetLists();
        Console.WriteLine($"UI Update: {lists.Count} Listen gefunden.");
        listView.ItemsSource = null;
        listView.ItemsSource = lists;
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
            bool confirm = await DisplayAlert("Löschen", $"Möchtest du die Liste '{list.Name}' wirklich löschen?", "Ja", "Nein");
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
