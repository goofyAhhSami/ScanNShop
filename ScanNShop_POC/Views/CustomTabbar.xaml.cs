namespace ScanNShop_POC.Views

{
    public partial class CustomTabbar : ContentView
    {
        public CustomTabbar()
        {
            InitializeComponent();

            bool isGuest = Preferences.Get("IsGuest", false);
            if (isGuest)
            {
                AiButton.IsVisible = false;
                AiLabel.IsVisible = false;

                // Grid-Position für ListsButton und ListsLabel anpassen
                Grid.SetColumn(ListsButton, 1);
                Grid.SetColumnSpan(ListsButton, 2);
                Grid.SetColumn(ListsLabel, 1);
                Grid.SetColumnSpan(ListsLabel, 2);
            }

        }

        private async void NavigateHome(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void NavigateAiPage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AiPage");
        }

        private async void NavigateListsPage(object sender, EventArgs e)
        {
            if (Shell.Current.CurrentPage is not ListsPage) // Prüfe, ob du schon dort bist
            {
                await Shell.Current.GoToAsync("ListsPage"); // Kein `//`, um die alte Instanz zu behalten
            }
        }

        private async void NavigateProfilePage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}
