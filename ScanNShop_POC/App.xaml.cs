using ScanNShop_POC.Database;

namespace ScanNShop_POC
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage(new LocalDbService())); //new AppShell();

        }
    }
}
