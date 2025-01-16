
using GroqSharp;
using GroqSharp.Models;
using ScanNShop_POC.Services;
using System.Collections.ObjectModel;

namespace ScanNShop_POC.Views;

public partial class AiPage : ContentPage
{
   

    public ObservableCollection<MessageObject> Messages
    {
        get { return (ObservableCollection<MessageObject>)GetValue(MessagesProperty); }
        set { SetValue(MessagesProperty, value); }
    }

    public static readonly BindableProperty MessagesProperty =
        BindableProperty.Create("Messages", typeof(ObservableCollection<MessageObject>), typeof(MainPage), null);

    private GroqCloudService _groqCloudService = new GroqCloudService();

    public AiPage()
    {
        InitializeComponent();

        Messages = new ObservableCollection<MessageObject>();

        this.GetResult("Du bist ein KI-Chatbot und bist hier, um meine Fragen zu beantworten.");

        this.BindingContext = this;
    }

    private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
    {

        var query = this.queryView.Text;
        this.queryView.Text = string.Empty;
        MessageObject userMessage = new MessageObject() { IsBot = false, Text = query };
        this.Messages.Add(userMessage);
        this.listView.ScrollTo(userMessage, ScrollToPosition.MakeVisible, true);
        this.GetResult(query);

    }

    private async void GetResult(string query)
    {
        this.busyIndicator.IsRunning = true;
        this.queryView.IsEnabled = false;
        this.sendImage.IsVisible = false;
        var reply = await _groqCloudService.ProcessTheUserInput(query);
        reply = reply.Replace("\n", "");
        MessageObject botMessage = new MessageObject() { IsBot = true, Text = reply };
        this.Messages.Add(botMessage);
        this.listView.ScrollTo(botMessage, ScrollToPosition.MakeVisible, true);
        this.sendImage.IsVisible = true;
        this.queryView.IsEnabled = true;
        this.busyIndicator.IsRunning = false;
    }


    private async void navigateBackToMainPage(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}