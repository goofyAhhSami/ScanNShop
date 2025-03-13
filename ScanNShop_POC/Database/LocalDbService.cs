using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanNShop_POC.Database
{
    public class LocalDbService
    {
        private const string DB_NAME = "local_database.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<Liste>();
            _connection.CreateTableAsync<Product>();  // Create table for Product
        }

        public async Task<List<Liste>> GetLists()
        {
            try
            {
                var lists = await _connection.Table<Liste>().ToListAsync();
                Console.WriteLine($"📋 Anzahl der Listen geladen: {lists.Count}");

                foreach (var list in lists)
                {
                    Console.WriteLine($"✅ Liste: {list.listId} - {list.Name}");
                }

                return lists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Abrufen der Listen: {ex.Message}");
                return new List<Liste>(); // Gibt eine leere Liste zurück, falls ein Fehler auftritt
            }
        }



        public async Task<Liste> GetById(int id)
        {
            return await _connection.Table<Liste>().Where(x => x.listId == id).FirstOrDefaultAsync();
        }

        public async Task Create(Liste liste)
        {
            try
            {
                if (_connection == null)
                {
                    Console.WriteLine("❌ SQLite-Verbindung ist NULL!");
                    return;
                }

                await _connection.InsertAsync(liste);
                Console.WriteLine($"✅ Liste '{liste.Name}' wurde erfolgreich gespeichert.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FEHLER in Create: {ex.Message}");
            }
        }

        public async Task DeleteAllListsAsync()
        {
            await _connection.DeleteAllAsync<Liste>(); // SQLite-Befehl, um alle Einträge zu löschen
        }


        public async Task Update(Liste liste)
        {
            await _connection.UpdateAsync(liste);
        }

        public async Task Delete(Liste liste)
        {
            await _connection.DeleteAsync(liste);
        }

        // Product operations
        public async Task<List<Product>> GetProductsByListId(int listId)
        {
            return await _connection.Table<Product>().Where(p => p.ListId == listId && !p.IsChecked).ToListAsync();
        }

        public async Task<List<Product>> GetCheckedProductsByListId(int listId)
        {
            return await _connection.Table<Product>().Where(p => p.ListId == listId && p.IsChecked).ToListAsync();
        }

        public async Task CreateProduct(Product product)
        {
            await _connection.InsertAsync(product);
        }
        public async Task UpdateProduct(Product product)
        {
            await _connection.UpdateAsync(product);
        }

        public async Task DeleteProduct(Product product)
        {
            await _connection.DeleteAsync(product);
        }

        public async Task<Liste> GetListByIdAsync(int listId)
        {
            return await _connection.Table<Liste>().FirstOrDefaultAsync(l => l.listId == listId);
        }

        public async Task DeleteListAsync(Liste list)
        {
            await _connection.DeleteAsync(list);
        }

        public async Task DebugDatabase()
        {
            var tables = await _connection.QueryAsync<dynamic>("PRAGMA table_info(Liste)");
            if (tables.Count == 0)
            {
                Console.WriteLine("❌ FEHLER: Die Tabelle 'Liste' existiert NICHT!");
            }
            else
            {
                Console.WriteLine("✅ Tabelle 'Liste' existiert mit " + tables.Count + " Spalten.");
            }
        }

        



    }
}
