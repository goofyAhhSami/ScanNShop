using ScanNShop_POC.Database;

namespace ScanNShop_POC.Views;

public partial class NewList : ContentPage
{
    private readonly LocalDbService _dbService;
    private int _editListId;
    public NewList(LocalDbService dbService,int editListId)
	{
		InitializeComponent();
        _dbService = dbService;
        _editListId = editListId;
    }

    private async void saveButton_Clicked(object sender, EventArgs e)
    {
        if (_editListId == 0)
        {
            await _dbService.Create(new Liste
            {
                Name = nameEntryField.Text
            });
        }
        else
        {
            await _dbService.Update(new Liste
            {
                ListId = _editListId,
                Name = nameEntryField.Text
            });
            _editListId = 0;
        }

        nameEntryField.Text = string.Empty;

        
        await Navigation.PopAsync();
        await saveList.ScaleTo(1.1, 150, Easing.CubicInOut);
        await saveList.ScaleTo(1, 150, Easing.CubicInOut);
        if (Application.Current.MainPage is NavigationPage navPage &&
         navPage.CurrentPage is MainPage mainPage)
        {
            await mainPage.UpdateListViewAsync();
        }

    }

    private void ContentPage_NavigatingFrom(object sender, NavigatingFromEventArgs e)
    {

    }

    private async void navigateBackToMainPage(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}