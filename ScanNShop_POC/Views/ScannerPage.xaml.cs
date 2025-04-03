using ScanNShop_POC.ProductModel;
using System.Net;
using System;
using ZXing.Net.Maui;
using Newtonsoft.Json;
using System.Collections.Generic;
using ScanNShop_POC.Database;

namespace ScanNShop_POC.Views;

public partial class ScannerPage : ContentPage
{

    private string lastBarcode;

    private readonly LocalDbService _dbService;
    private readonly int _listId;
    public ScannerPage(LocalDbService dbService, int listId)
    {
        InitializeComponent();
        _dbService = dbService;
        _listId = listId;
        /* barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
         {
             AutoRotate = true,
             Formats = ZXing.Net.Maui.BarcodeFormat.Ean8 | ZXing.Net.Maui.BarcodeFormat.Ean13
         };*/
        barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions { AutoRotate = true, Formats = ZXing.Net.Maui.BarcodeFormat.Ean13 };


    }



    private void barcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {   
        var first = e.Results?.FirstOrDefault();
        if (lastBarcode != first.Value)
        {
            lastBarcode = first.Value;
            if (first is null)
                return;

            Dispatcher.DispatchAsync(async () =>
            {
                await GetScannedProductInfo(first.Value);
            });
        }

    }

    private async Task GetScannedProductInfo(string barcodeValue)
    {
        using (WebClient webClient = new WebClient())
        {
            string api_key = "zy3pfapj8trmgczo4ql29echrgchmm";
            string url = $"https://api.barcodelookup.com/v3/products?barcode={barcodeValue}&formatted=y&key={api_key}";

            try
            {
                var data = await webClient.DownloadStringTaskAsync(url);
                var product = JsonConvert.DeserializeObject<RootObject>(data).products[0];

                string name = product.title; // Produktname
                string barcode = product.barcode_number;

                Console.WriteLine($"Title: {name}");
                Console.WriteLine($"Barcode Number: {barcode}");
                Console.WriteLine("Entire Response:");
                Console.WriteLine(data);

          
                bool addToList = await DisplayAlert("Produkt hinzuf¸gen",
                    $"Wollen Sie das Produkt '{name}' zur Liste hinzuf¸gen?",
                    "Ja",
                    "Nein");

                if (addToList)
                {
                    await AddProductToList(name, barcode);
                    await Navigation.PopAsync();
                }
                else
                {
                    lastBarcode = ""; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    private async Task AddProductToList(string name, string barcode)
    {
        var product = new Database.Product
        {
            Name = name,
            Quantity = 1,
            ListId = _listId, 
            IsChecked = false
        };

        await _dbService.CreateProduct(product);
        await DisplayAlert("Erfolgreich", $"Das Produkt '{name}' wurde zur Liste hinzugef¸gt.", "OK");
    }

    private async void CloseScanner(object sender, EventArgs e)
    {
        try
        {
            // Falls du Navigation ¸ber Shell nutzt
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Schlieﬂen des Scanners: {ex.Message}");
        }
    }

}