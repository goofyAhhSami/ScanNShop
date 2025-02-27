using ScanNShop_POC.Database;
using Microsoft.Maui.Controls;

namespace ScanNShop_POC.Views;

public partial class ShoppingView : ContentPage
{
    private readonly LocalDbService _dbService;
    private readonly int _listId;

    public ShoppingView(LocalDbService dbService, int listId)
    {
        InitializeComponent();
        _dbService = dbService;
        _listId = listId;
        LoadProducts();

      
    }

    public async void LoadProducts()
    {
        var uncheckedProducts = await _dbService.GetProductsByListId(_listId);
        var checkedProducts = await _dbService.GetCheckedProductsByListId(_listId);

        unCheckedProductsListView.ItemsSource = uncheckedProducts;
        checkedProductsListView.ItemsSource = checkedProducts;

        if (uncheckedProducts.Count == 0)
        {
            await ShowCompletionPopup();
        }
        else
        {
            // Ensure the popup is hidden if products are present
            PopupOverlay.IsVisible = false;
        }


    }

    private async void Product_ItemTapped(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Product product)
        {
            product.IsChecked = !product.IsChecked;
            await _dbService.UpdateProduct(product);
            LoadProducts();
        }
    }

    private async Task ShowCompletionPopup()
    {
        PopupOverlay.IsVisible = true; // Popup sichtbar machen
    }

    private async void ClosePopup(object sender, EventArgs e)
    {
        PopupOverlay.IsVisible = false; // Popup ausblenden
        await Navigation.PopAsync(true);
      
    }


    private async void IncreaseQuantity_Clicked(object sender, EventArgs e)
    {
        var product = (Product)((Button)sender).CommandParameter;
        product.Quantity++;
        await _dbService.UpdateProduct(product);
        LoadProducts();
    }

    private async void DecreaseQuantity_Clicked(object sender, EventArgs e)
    {
        var product = (Product)((Button)sender).CommandParameter;
        if (product.Quantity > 1)
        {
            product.Quantity--;
            await _dbService.UpdateProduct(product);
        }
        else
        {
            await _dbService.DeleteProduct(product);
        }
        LoadProducts();
    }

    private async void navigateBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync(true);
    }

}
