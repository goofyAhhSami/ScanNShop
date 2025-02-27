namespace ScanNShop_POC.Views

{
    public partial class CustomTabbar : ContentView
    {
        public CustomTabbar()
        {
            InitializeComponent();
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
            await Shell.Current.GoToAsync("//ListsPage");
        }

        private async void NavigateProfilePage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}
