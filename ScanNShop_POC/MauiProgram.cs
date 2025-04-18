﻿using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using ScanNShop_POC.Database;
using ZXing.Net.Maui.Controls;
using Syncfusion.Maui.Core.Hosting;
using CommunityToolkit.Maui;
using ScanNShop_POC.Services;

namespace ScanNShop_POC
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Anton-Regular.ttf", "Anton");
                    fonts.AddFont("Pacifico-Regular.ttf", "Pacifico");
                    fonts.AddFont("Nunito-VariableFont_wght.ttf", "Nunito");
                    fonts.AddFont("VarelaRound-Regular.ttf", "Varela");
                }).UseBarcodeReader();
            builder.ConfigureSyncfusionCore();
            //builder.Services.AddSingleton<LocalDbService>();
            // LocalDbService als Singleton registrieren
            builder.Services.AddSingleton(LocalDbService.Instance);
            builder.Services.AddSingleton(ApiService.Instance);
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}


