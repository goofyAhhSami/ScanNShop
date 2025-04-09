using Newtonsoft.Json;
using ScanNShop_POC.Database;
using ScanNShop_POC.DTO;
using ScanNShop_POC.Services;

namespace ScanNShop_POC.Views;

public partial class ShareListPopup : ContentPage
{
    private readonly int _listId;
    private List<User> _allUsers = new();
    private List<User> _filteredUsers = new();

    public ShareListPopup(int listId)
    {
        InitializeComponent();
        _listId = listId;
        LoadUsers();
    }

    private async void LoadUsers()
    {
        var api = ApiService.Instance;
        _allUsers = await api.GetAllUsers();
        _filteredUsers = new List<User>(); // Anfangs leer!
        usersCollection.ItemsSource = _filteredUsers;
    }


    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.Trim().ToLower() ?? string.Empty;

        if (searchText.Length < 1)
        {
            usersCollection.ItemsSource = new List<User>(); // nichts anzeigen
            return;
        }

        _filteredUsers = _allUsers
            .Where(u =>
                (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Contains(searchText)) ||
                (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(searchText))
            )
            .ToList();

        usersCollection.ItemsSource = _filteredUsers;
    }

    private async void ShareButton_Clicked(object sender, EventArgs e)
    {
        var selectedUsers = _allUsers.Where(u => u.IsSelected).ToList();

        if (!selectedUsers.Any())
        {
            await DisplayAlert("Hinweis", "Bitte wählen Sie mindestens einen Benutzer aus.", "OK");
            return;
        }

        var api = ApiService.Instance;
        var userIds = selectedUsers.Select(u => u.UserId).ToList();

        bool success = await api.ShareListWithUsers(_listId, userIds);
        if (success)
        {
            await ShowShareSuccessAnimation();
            await Task.Delay(1500);
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Fehler", "Teilen fehlgeschlagen.", "OK");
        }
    }

    private async Task ShowShareSuccessAnimation()
    {
        if (shareSuccessNotification == null) return;

        shareSuccessNotification.IsVisible = true;
        shareSuccessNotification.Opacity = 0;
        shareSuccessNotification.TranslationY = 100;

        // Slide + Fade In
        await Task.WhenAll(
            shareSuccessNotification.FadeTo(1, 300),
            shareSuccessNotification.TranslateTo(0, 0, 300, Easing.CubicOut)
        );

        await Task.Delay(1500);

        // Slide + Fade Out
        await Task.WhenAll(
            shareSuccessNotification.FadeTo(0, 300),
            shareSuccessNotification.TranslateTo(0, 100, 300, Easing.CubicIn)
        );

        shareSuccessNotification.IsVisible = false;
    }



    private async void navigateBack(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
