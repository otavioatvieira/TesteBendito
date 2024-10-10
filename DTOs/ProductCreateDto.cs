using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bendito.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; } 
    }
}