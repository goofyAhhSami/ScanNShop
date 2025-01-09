using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanNShop_POC.ProductModel
{
    public class Product
    {
        public String barcode_number { get; set; }
        public String barcode_formats { get; set; }
        public String mpn { get; set; }
        public String model { get; set; }
        public String asin { get; set; }
        public String title { get; set; }
        public String category { get; set; }
        public String manufacturer { get; set; }
        public String brand { get; set; }
        public IList<object> contributors { get; set; }
        public String age_group { get; set; }
        public String ingredients { get; set; }
        public String nutrition_facts { get; set; }
        public String color { get; set; }
        public String format { get; set; }
        public String multipack { get; set; }
        public String size { get; set; }
        public String length { get; set; }
        public String width { get; set; }
        public String height { get; set; }
        public String weight { get; set; }
        public String release_date { get; set; }
        public String description { get; set; }
        public IList<string> features { get; set; }
        public IList<string> images { get; set; }
        public IList<Store> stores { get; set; }
        public IList<Review> reviews { get; set; }
    }
}
