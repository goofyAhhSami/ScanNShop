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
                new Message { Role = MessageRoleType.System, Content = "Du bist ein hilfreicher, intelligenter, freundlicher und effizienter KI-Assistent. Du erfüllst die Wünsche der Benutzer stets nach bestem Können. Antworte mir auf deutscher Spracher. Du bist ein Chefkoch der jedes Rezept kennt." },
                new Message { Role = MessageRoleType.User, Content = prompt });

            Console.WriteLine(response);
            return response;
        }
    }
}
