
using Android.App.Job;
using GroqSharp;
using GroqSharp.Models;
using Newtonsoft.Json;
using ScanNShop_POC.Database;
using ScanNShop_POC.DTO;
using ScanNShop_POC.Services;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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
        reply = ConvertMarkdownToFormattedText(reply);
        MessageObject botMessage = new MessageObject() { IsBot = true, Text = reply };
        this.Messages.Add(botMessage);
        this.listView.ScrollTo(botMessage, ScrollToPosition.MakeVisible, true);
        await HandleAiReply(reply);
        this.sendImage.IsVisible = true;
        this.queryView.IsEnabled = true;
        this.busyIndicator.IsRunning = false;
    }

    private string ConvertMarkdownToFormattedText(string input)
    {
        // Fett markieren (ersetze **text** mit Unicode-Zeichen für Fett)
        input = Regex.Replace(input, @"\*\*(.*?)\*\*", match => "𝗧𝗘𝗫𝗧".Replace("𝗧𝗘𝗫𝗧", match.Groups[1].Value));
        input = Regex.Replace(input, @"(?m)^\* (.+)", "• $1");
        return input;
    }


    private async void navigateBackToMainPage(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async Task HandleAiReply(string reply)
    {
        try
        {
            // 📌 Titel extrahieren
            string listName = Regex.Match(reply, @"Name:\s*(.+)", RegexOptions.IgnoreCase)?.Groups[1].Value.Trim();

            if (string.IsNullOrWhiteSpace(listName))
            {
                Console.WriteLine("❌ Kein Listenname gefunden.");
                return;
            }

            var _dbService = LocalDbService.Instance;

            var neueListe = new Liste { Name = listName };
            await _dbService.Create(neueListe);
            Console.WriteLine($"✅ Lokale Liste '{listName}' erstellt.");

            int listId = neueListe.ListId;

            // 🌐 Sync mit Server
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var userRaw = Preferences.Get("User", null);
                var userObj = JsonConvert.DeserializeObject<User>(userRaw);

                var api = ApiService.Instance;
                var newListId = await api.InsertList(new CreateListDto
                {
                    Name = listName,
                    UserId = userObj.UserId
                });

                if (newListId.HasValue)
                {
                    await _dbService.Delete(neueListe);
                    listId = newListId.Value;
                    await _dbService.Create(new Liste
                    {
                        ListId = listId,
                        Name = listName,
                        CreationDate = DateTime.UtcNow
                    });
                    Console.WriteLine($"🌍 Server-ListId {listId} gespeichert");
                }
            }

            // 📌 Produkte extrahieren (Zeilen mit • oder -)
            var productMatches = Regex.Matches(reply, @"[•\-]\s*(.+?)\s*(\d+)x", RegexOptions.Multiline);

            foreach (Match match in productMatches)
            {
                string name = match.Groups[1].Value.Trim();
                int quantity = int.Parse(match.Groups[2].Value);

                var product = new Product
                {
                    ListId = listId,
                    Name = name,
                    Quantity = quantity,
                    IsChecked = false
                };

                await _dbService.CreateProduct(product);

                // 🔄 Sync Produkt mit Server
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var serverProductId = await ApiService.Instance.InsertSingleProduct(product);

                    if (serverProductId.HasValue)
                    {
                        await _dbService.DeleteProduct(product);
                        await _dbService.CreateProduct(new Product
                        {
                            ProductId = serverProductId.Value,
                            ListId = listId,
                            Name = name,
                            Quantity = quantity,
                            IsChecked = false
                        });
                    }
                }
            }

            Console.WriteLine("✅ Einkaufsliste und Produkte aus AI erstellt.");

            // ✅ Erfolgsmeldung im Chat anzeigen
            Device.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(new MessageObject
                {
                    IsBot = true,
                    Text = $"✅ Liste wurde erstellt: *{listName}* mit {productMatches.Count} Produkten."
                });

                listView.ScrollTo(Messages.Last(), ScrollToPosition.MakeVisible, true);
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler bei HandleAiReply: {ex.Message}");
        }
    }

}