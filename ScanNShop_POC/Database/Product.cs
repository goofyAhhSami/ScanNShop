using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanNShop_POC.Database
{
    [Table("product")]
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int ProductId { get; set; }

        [Indexed]
        public int ListId { get; set; }  // Foreign key to link with Liste

        public string Name { get; set; }

        public int Quantity { get; set; }

        public bool IsChecked { get; set; }  // Indicates if the product is "purchased" or "checked"
    }
}
