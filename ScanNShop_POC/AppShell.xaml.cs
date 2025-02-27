using ScanNShop_POC.Database;
using ScanNShop_POC.Views;

namespace ScanNShop_POC
{
    public partial class AppShell : Shell
    {
        private readonly LocalDbService _dbService;

        public AppShell(LocalDbService dbService)
        {
            InitializeComponent();
            _dbService = dbService;

            Routing.RegisterRoute(nameof(ListsPage), typeof(ListsPage));
            Routing.RegisterRoute(nameof(ListEdit), typeof(ListEdit));
        }



    }
}
