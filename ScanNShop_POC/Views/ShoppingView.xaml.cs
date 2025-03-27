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
        var allProducts = await _dbService.GetAllProductsByListId(_listId);

        var sorted = allProducts.OrderBy(p => p.IsChecked).ToList();
        productsCollectionView.ItemsSource = sorted;

        emptyListLabel.IsVisible = !sorted.Any(p => !p.IsChecked);
        UpdateShoppingStatus(sorted);
    }

    private void UpdateShoppingStatus(List<Product> products)
    {
        int remaining = products.Count(p => !p.IsChecked);
        int done = products.Count(p => p.IsChecked);

        if (remaining == 0 && done > 0)
            shoppingStatusLabel.Text = "Einkauf abgeschlossen!";
        else if (remaining > 0)
            shoppingStatusLabel.Text = $"Einkauf läuft – {remaining} Artikel übrig";
        else
            shoppingStatusLabel.Text = "Keine Produkte in der Liste.";
    }

    private async void Product_ItemTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Product product)
        {
            product.IsChecked = !product.IsChecked;
            await _dbService.UpdateProduct(product);
            LoadProducts();
        }
    }

    private async void IncreaseQuantity_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
        {
            product.Quantity++;
            await _dbService.UpdateProduct(product);
            LoadProducts();
        }
    }

    private async void DecreaseQuantity_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Product product)
        {
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
    }

    private async void navigateBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync(true);
    }
}
