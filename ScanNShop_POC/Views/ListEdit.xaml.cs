using ScanNShop_POC.Database;
using ScanNShop_POC.Views;

namespace ScanNShop_POC.Views;

[QueryProperty(nameof(ListId), "listId")]
public partial class ListEdit : ContentPage
{
    private readonly LocalDbService _dbService;
    private int _listId;

    public ListEdit()
    {
        InitializeComponent();
      
        _dbService = LocalDbService.Instance; // Singleton-Instanz verwenden

        //shoppingView = new ShoppingView(dbService, _listId);
    }


    public string ListId
    {
        set
        {
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id))
            {
                _listId = id;
                LoadListData();
            }
        }
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
            LoadListData();

            if (productAddedNotificationFrame != null && productAddedNotificationFrame.Handler != null)
            {
                await productAddedNotificationFrame.FadeTo(1, 500);
                await Task.Delay(2000);

                if (productAddedNotificationFrame != null && productAddedNotificationFrame.Handler != null)
                {
                    await productAddedNotificationFrame.FadeTo(0, 500);
                }
            }

        }
    }

    private async void LoadListData()
    {
        var list = await _dbService.GetListByIdAsync(_listId);
        if (list != null)
        {
            Title = list.Name;
            listNameLabel.Text = $"{list.Name}";

            // Gesamtanzahl der Produkte
            int totalProducts = await GetProductCount(_listId);
            productCountLabel.Text = $"{totalProducts}";

            // Gekaufte Produkte
            int purchasedProducts = await GetPurchasedProductCount(_listId);
            purchasedCountLabel.Text = $"{purchasedProducts}";

            // Übrige Produkte berechnen
            int remainingProducts = totalProducts - purchasedProducts;
            remainingCountLabel.Text = $"{remainingProducts}";

            // Erstellungsdatum anzeigen
            creationDateLabel.Text = list.CreationDate != default
                ? $"{list.CreationDate:dd.MM.yyyy}"
                : "Unbekannt";
        }
        else
        {
            await DisplayAlert("Fehler", "Liste nicht gefunden.", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }




    private async Task<int> GetProductCount(int listId)
    {
        return await _dbService.GetProductCountAsync(listId);
    }

    private async Task<int> GetPurchasedProductCount(int listId)
    {
        return await _dbService.GetPurchasedProductCountAsync(listId);
    }





    private async void navigateBack(object sender, EventArgs e)
    {
        try
        {
            //if (productAddedNotificationFrame != null && productAddedNotificationFrame.Handler != null)
           // {
             //   productAddedNotificationFrame.IsVisible = false;
          //  }

            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.GoToAsync("..", true);
            }
            else
            {
                Console.WriteLine("⚠ Navigation nicht möglich: Keine vorherige Seite in der Shell-Navigation.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler bei der Navigation: {ex.Message}");
        }
    }



    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        //if (productAddedNotificationFrame != null && productAddedNotificationFrame.Handler != null)
       // {
        //    productAddedNotificationFrame.IsVisible = false;
       // }

        Console.WriteLine("🔄 ListEdit wird verlassen, UI-Elemente deaktiviert.");
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
                await _dbService.Delete(listToDelete);
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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_dbService == null)
        {
            Console.WriteLine("⚠ _dbService ist NULL in ListsPage!");
            return;
        }

    }


}