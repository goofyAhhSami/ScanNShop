using ScanNShop_POC.MVVM.View;

namespace ScanNShop_POC
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void createNewList(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NewList());
        }

        private void editList(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ListEdit());
        }
    }

}
