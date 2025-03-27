using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace ScanNShop_POC.Database
{
    public class LocalDbService
    {
        private const string DB_NAME = "local_database.db3";
        private static LocalDbService _instance;
        private readonly SQLiteAsyncConnection _connection;

        // Singleton-Instanz abrufen
        public static LocalDbService Instance => _instance ??= new LocalDbService();

        private LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            await _connection.CreateTableAsync<Liste>();
            await _connection.CreateTableAsync<Product>();

            // Falls eine alte Datenbank existiert, prüfe, ob das Feld "CreationDate" vorhanden ist
            var tableInfo = await _connection.GetTableInfoAsync("liste");
            if (!tableInfo.Exists(c => c.Name == "creation_date"))
            {
                await _connection.ExecuteAsync("ALTER TABLE liste ADD COLUMN creation_date TEXT");
            }
        }

        public async Task<List<Liste>> GetLists()
        {
            try
            {
                return await _connection.Table<Liste>().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Abrufen der Listen: {ex.Message}");
                return new List<Liste>();
            }
        }

        public async Task<Liste> GetListByIdAsync(int listId)
        {
            return await _connection.Table<Liste>().FirstOrDefaultAsync(l => l.listId == listId);
        }

        public async Task Create(Liste liste)
        {
            liste.CreationDate = DateTime.UtcNow; 
            await _connection.InsertAsync(liste);
        }
        public async Task Update(Liste liste) => await _connection.UpdateAsync(liste);
        public async Task Delete(Liste liste) => await _connection.DeleteAsync(liste);
        public async Task DeleteAllListsAsync() => await _connection.DeleteAllAsync<Liste>();

        public async Task<List<Product>> GetCheckedProductsByListId(int listId)
        {
            return await _connection.Table<Product>().Where(p => p.ListId == listId && p.IsChecked).ToListAsync();
        }

        public async Task<List<Product>> GetProductsByListId(int listId)
        {
            return await _connection.Table<Product>().Where(p => p.ListId == listId && !p.IsChecked).ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsByListId(int listId)
        {
            return await _connection.Table<Product>()
                                    .Where(p => p.ListId == listId)
                                    .ToListAsync();
        }


        public async Task CreateProduct(Product product) => await _connection.InsertAsync(product);
        public async Task UpdateProduct(Product product) => await _connection.UpdateAsync(product);
        public async Task DeleteProduct(Product product) => await _connection.DeleteAsync(product);

        public async Task<int> GetProductCountAsync(int listId)
        {
            return await _connection.Table<Product>().Where(p => p.ListId == listId).CountAsync();
        }

        public async Task<int> GetPurchasedProductCountAsync(int listId)
        {
            return await _connection.Table<Product>().Where(p => p.ListId == listId && p.IsChecked).CountAsync();
        }



    }
}
