using GroqSharp.Models;
using GroqSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScanNShop_POC.Services
{



    internal class GroqCloudService
    {
        private string apiKey = "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";

        private string apiModel = "llama3-70b-8192";

        public GroqCloudService()
        {

        }

        internal async Task<string> ProcessTheUserInput(string prompt)
        {
            IGroqClient groqClient = new GroqClient(apiKey, apiModel)
            .SetTemperature(0.5)
            .SetMaxTokens(256)
            .SetTopP(1)
            .SetStop("NONE");

            var response = await groqClient.CreateChatCompletionAsync(
                new Message { Role = MessageRoleType.System, Content = "Du bist ein hilfreicher, intelligenter, freundlicher und effizienter KI-Assistent. Antworte mir auf deutscher Spracher. Du kannst auch Einkaufslisten erstellen, wenn man dich fragt. Generiere auch eine Name für die Einkaufsliste und schreiben es so hin (Name: {name von liste}) also vor dem Namen der Einkaufsliste soll Name: stehen und formatiere die Antworten immer gleich wenn du eine Einkaufsliste erstellst also es sollen alle Produkte untereinander mit Aufzählungspunkte da stehen. Schreibe bitte auch keine Einheiten sondern wirklich nur die Produkte und Menge dahinter als Zahl und dann x hinter der Zahl. Die Menge soll nicht als gramm oder so angezeigt werden sondern wenn es z.b 500 Gramm Spaghetti ist dann schreibe nur 1x" },
                new Message { Role = MessageRoleType.User, Content = prompt });

            Console.WriteLine(response);
            return response;
        }
    }
}
