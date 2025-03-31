using ScanNShop_POC.Database;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ScanNShop_POC.DTO;

namespace ScanNShop_POC.Services
{
    public class ApiService
    {

        private static ApiService _instance;
        public static ApiService Instance => _instance ??= new ApiService();

        private readonly HttpClient _httpClient;

        private ApiService()
        {
            Console.WriteLine("ApiService gestartet.");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:5254/api/")
            };
        }


        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<bool> Register(string email, string username, string password)
        {
            var user = new { Email = email, Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Auth/register", content);

            Console.WriteLine($"[Register] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Registrierung fehlgeschlagen: {errorMessage}");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<string> Login(string username, string password)
        {
            var loginRequest = new { Username = username, Password = password };
            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Auth/login", content);

            Console.WriteLine($"[Login] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseData);
                return tokenResponse.token;
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Login fehlgeschlagen: {error}");

            return null;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var response = await _httpClient.GetAsync($"Data/users/by-username/{username}");

            Console.WriteLine($"[GetUserByUsername] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Abrufen des Benutzers: {error}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<User>(json);
        }

        public async Task<List<Liste>> GetListsByUserId(int userId)
        {
            var response = await _httpClient.GetAsync($"Data/users/{userId}/lists");

            Console.WriteLine($"[GetListsByUserId] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Abrufen der Listen: {error}");
                return new List<Liste>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Liste>>(json);
        }

        public async Task<List<Product>> GetProductsByListIds(List<int> listIds)
        {
            var json = JsonConvert.SerializeObject(listIds);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Data/products/by-list-ids", content);

            Console.WriteLine($"[GetProductsByListIds] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Abrufen der Produkte: {error}");
                return new List<Product>();
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Product>>(result);
        }

        public async Task<int?> InsertList(CreateListDto dto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Data/lists", content);

            Console.WriteLine($"[InsertList] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
                return (int?)result.listId; // exakt wie Server zurückgibt
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Erstellen der Liste: {error}");
                return null;
            }
        }


        public async Task<bool> InsertProducts(List<Product> products)
        {
            var content = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Data/products", content);

            Console.WriteLine($"[InsertProducts] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Hinzufügen von Produkten: {error}");
            }

            return response.IsSuccessStatusCode;
        }
    

    public async Task<bool> DeleteListFromServer(int listId)
        {
            var response = await _httpClient.DeleteAsync($"Data/lists/{listId}");
            Console.WriteLine($"[DeleteListFromServer] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Löschen der Liste auf dem Server: {error}");
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateProduct(Product product)
        {
            Console.WriteLine("Menge: " + product.Quantity);
            Console.WriteLine("Listid: " + product.ListId);
            Console.WriteLine("ProductId: " + product.ProductId);
            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ListId = product.ListId, // ✅ wichtig!
                Name = product.Name,
                Quantity = product.Quantity,
                IsChecked = product.IsChecked
            };

            var content = new StringContent(JsonConvert.SerializeObject(new List<ProductDto> { productDto }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("Data/products/update", content);

            Console.WriteLine($"[UpdateProduct] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Fehler beim Aktualisieren des Produkts: {error}");
            }

            return response.IsSuccessStatusCode;
        }


        public async Task<int?> InsertSingleProduct(Product product)
        {
            var dto = new ProductDto
            {
                ListId = product.ListId,
                Name = product.Name,
                Quantity = product.Quantity,
                IsChecked = product.IsChecked
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Data/products/single", content);

            Console.WriteLine($"[InsertSingleProduct] StatusCode: {(int)response.StatusCode} - {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
                return (int?)result.productId;
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Fehler beim Hinzufügen eines Produkts: {error}");
            return null;
        }




    }
}
