using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanNShop_POC.Database
{
    [Table("liste")]
    public class Liste
    {
       
        [PrimaryKey]
        public int ListId { get; set; } // statt listId


        [Column("name")]
        public string Name { get; set; }

        [Column("creation_date")]
        public DateTime CreationDate { get; set; } 


    }
}
