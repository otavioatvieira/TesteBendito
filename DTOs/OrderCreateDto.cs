using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bendito.DTOs
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; }
    }

    public class OrderItemCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}