using ScanNShop_POC.Database;
using ScanNShop_POC.Views;

namespace ScanNShop_POC
{
    public partial class App : Application
    {
        public App()
        {   

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY3NTIxOEAzMjM4MmUzMDJlMzBTd0JpRGVsNVV5WWMxdHlxaGZwVDlmb0JhNmhCcjdab2NjKytqSjFDNjdrPQ==");

  
            InitializeComponent();

            MainPage = new NavigationPage(new LogInPage());
        }
    }
}
