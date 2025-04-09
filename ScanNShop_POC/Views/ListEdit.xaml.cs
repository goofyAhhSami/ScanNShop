using ScanNShop_POC.Database;
using ScanNShop_POC.Views;
using Microsoft.Maui.Networking; // Wichtig für Connectivity
using System.Windows.Input;
using ScanNShop_POC.Services;

namespace ScanNShop_POC.Views;

[QueryProperty(nameof(ListId), "listId")]
public partial class ListEdit : ContentPage
{
    private readonly LocalDbService _dbService;
    private int _listId;
    public ICommand NavigateBackCommand { get; }

    public ListEdit()
    {
        InitializeComponent();

        _dbService = LocalDbService.Instance; // Singleton-Instanz verwenden

        //shoppingView = new ShoppingView(dbService, _listId);
        NavigateBackCommand = new Command(async () => await navigateBack());
        BindingContext = this;
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
            var localProduct = new Product
            {
                ListId = _listId,
                Name = productEntryField.Text,
                Quantity = 1,
                IsChecked = false
            };

            // 🟡 Lokale Version einfügen
            await _dbService.CreateProduct(localProduct);

            // 🟢 Server Sync
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var api = ApiService.Instance;
                var serverProductId = await api.InsertSingleProduct(localProduct);
                Console.WriteLine(serverProductId.ToString());
                if (serverProductId.HasValue)
                {
                    // ❌ lokale (falsche ID) löschen
                    await _dbService.DeleteProduct(localProduct);

                    // ✅ neu mit Server-ID einfügen
                    var synced = new Product
                    {
                        ProductId = serverProductId.Value,
                        ListId = localProduct.ListId,
                        Name = localProduct.Name,
                        Quantity = localProduct.Quantity,
                        IsChecked = localProduct.IsChecked
                    };

                    await _dbService.CreateProduct(synced);
                    Console.WriteLine($"✅ Produkt mit Server-Id gespeichert: {serverProductId.Value}");
                }
            }

            productEntryField.Text = string.Empty;
            LoadListData();

            if (productAddedNotificationFrame?.Handler != null)
            {
                await productAddedNotificationFrame.FadeTo(1, 500);
                await Task.Delay(2000);
                if (productAddedNotificationFrame?.Handler != null)
                    await productAddedNotificationFrame.FadeTo(0, 500);
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



    private bool _isNavigating = false;

    private async Task navigateBack()
    {
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            if (productAddedNotificationFrame?.Handler != null)
            {
                productAddedNotificationFrame.IsVisible = false;
            }

            if (Shell.Current.CurrentPage is not ListsPage) // Prüfe, ob du schon dort bist
            {
                await Shell.Current.GoToAsync("ListsPage"); // Kein `//`, um die alte Instanz zu behalten
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
        finally
        {
            _isNavigating = false;
        }
    }



    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }


    private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        UpdateScannerButtonState();
    }

    private void UpdateScannerButtonState()
    {
        bool isOnline = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        ScannerButton.IsEnabled = isOnline;
        ScannerButton.Opacity = isOnline ? 1.0 : 0.5;
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
        if (!confirm) return;

        var listToDelete = await _dbService.GetListByIdAsync(_listId);
        if (listToDelete == null)
        {
            await DisplayAlert("Fehler", "Liste nicht gefunden.", "OK");
            return;
        }

        // 🌐 Versuche Server-Löschung, wenn Internet vorhanden
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                var api = ApiService.Instance;
                var success = await api.DeleteListFromServer(listToDelete.ListId);

                if (!success)
                    Console.WriteLine("⚠ Liste konnte auf dem Server nicht gelöscht werden.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Löschen auf dem Server: {ex.Message}");
            }
        }

        // 🗑 Lokale Liste löschen
        await _dbService.Delete(listToDelete);

        // 🔄 MainPage & ListsPage benachrichtigen
        MessagingCenter.Send(this, "ListDeleted");
        MessagingCenter.Send(this, "ListDeletedFromListsPage");

        // Navigation zurück
        await Shell.Current.GoToAsync("ListsPage");
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_dbService == null)
        {
            Console.WriteLine("⚠ _dbService ist NULL in ListsPage!");
            return;
        }

        UpdateScannerButtonState();
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
    }

    private async void navigateBack(object sender, EventArgs e)
    {
        try
        {
            //if (productAddedNotificationFrame != null && productAddedNotificationFrame.Handler != null)
            // {
            //   productAddedNotificationFrame.IsVisible = false;
            //  }

            if (Navigation.NavigationStack.Count > 1)
            {
               // await Navigation.PopAsync(); // ← verwendet NavigationStack, nicht Shell
                await Shell.Current.GoToAsync("ListsPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//ListsPage");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler bei der Navigation: {ex.Message}");
        }
    }

    private async void OpenSharePopup(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShareListPopup(_listId));
    }

}