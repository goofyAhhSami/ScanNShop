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
        private const string DB_NAME = "local_db.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _connection.CreateTableAsync<Liste>();
            _connection.CreateTableAsync<Product>();  // Create table for Product
        }

        public async Task<List<Liste>> GetLists()
        {
            return await _connection.Table<Liste>().ToListAsync();
        }

        public async Task<Liste> GetById(int id)
        {
            return await _connection.Table<Liste>().Where(x => x.listId == id).FirstOrDefaultAsync();
        }

        public async Task Create(Liste liste)
        {
            await _connection.InsertAsync(liste);
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
    }
}
