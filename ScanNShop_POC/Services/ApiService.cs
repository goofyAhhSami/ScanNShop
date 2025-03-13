using ScanNShop_POC.Database;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScanNShop_POC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            Console.WriteLine("ApiService");
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://10.0.2.2:5254/api/Auth/") // Die Basis-URL für Authentifizierung
            };
        }

        // Registrierung
        public async Task<bool> Register(string email, string username, string password)
        {
            var user = new
            {
                Email = email,
                Username = username,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("register", content);


            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Registrierung fehlgeschlagen: {errorMessage}");
            }

            return response.IsSuccessStatusCode;
        }

        // Login
        public async Task<string> Login(string username, string password)
        {
            var loginRequest = new
            {
                Username = username,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseData);
                return tokenResponse.token;
            }
            return null;
        }
    }
}
