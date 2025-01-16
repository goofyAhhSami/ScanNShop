using ScanNShop_POC.Database;
using ScanNShop_POC.Views;

namespace ScanNShop_POC.Views;

public partial class ListEdit : ContentPage
{
    private readonly LocalDbService _dbService;
    private readonly int _listId;

    public ListEdit(LocalDbService dbService, int listId)
    {
        InitializeComponent();
        _dbService = dbService;
        _listId = listId;

        //shoppingView = new ShoppingView(dbService, _listId);
    }

    private async void AddProductButton_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(productEntryField.Text))
        {
            var product = new Product
            {
                ListId = _listId,
                Name = productEntryField.Text,
                Quantity = 1,
                IsChecked = false
            };
            await _dbService.CreateProduct(product);
            productEntryField.Text = string.Empty;

            await productAddedNotificationFrame.FadeTo(1, 500); // Blendet die Benachrichtigung ein
            await Task.Delay(2000); // Wartezeit
            await productAddedNotificationFrame.FadeTo(0, 500); // Blendet die Benachrichtigung aus
        }
    }

   

    private async void navigateBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync(true);
    }

   

    private async void OpenScannerPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ScannerPage(_dbService, _listId), true);
    }

    private async void startShopping(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShoppingView(_dbService, _listId));
    }

    // Methode zum Löschen der Liste
    private async void DeleteListButton_Clicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Bestätigung", "Möchten Sie diese Liste wirklich löschen?", "Ja", "Abbrechen");
        if (confirm)
        {
            var listToDelete = await _dbService.GetListByIdAsync(_listId);
            if (listToDelete != null)
            {
                await _dbService.DeleteListAsync(listToDelete);
                // Nachricht senden, um die ListView auf der MainPage zu aktualisieren
                MessagingCenter.Send(this, "ListDeleted");
                await Navigation.PopAsync(true); // Zurück zur vorherigen Seite
            }
            else
            {
                await DisplayAlert("Fehler", "Liste nicht gefunden.", "OK");
            }
        }
    }

}