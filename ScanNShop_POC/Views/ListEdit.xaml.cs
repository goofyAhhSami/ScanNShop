using ScanNShop_POC.Database;

namespace ScanNShop_POC;

public partial class ListEdit : ContentPage
{
    private readonly LocalDbService _dbService;
    private readonly int _listId;

    public ListEdit(LocalDbService dbService, int listId)
    {
        InitializeComponent();
        _dbService = dbService;
        _listId = listId;
        LoadProducts();
    }

    private async void LoadProducts()
    {
        unCheckedProductsListView.ItemsSource = await _dbService.GetProductsByListId(_listId);
        checkedProductsListView.ItemsSource = await _dbService.GetCheckedProductsByListId(_listId);
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
            LoadProducts();
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
            //var product = (Product)e.Item;
      
    }

    private async void navigateBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
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

    
}