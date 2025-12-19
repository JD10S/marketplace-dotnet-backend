using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Marketplace.Entities.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } 
        public int Stock { get; set; } 
        [Column("imageurl")]
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } 

    }
}
