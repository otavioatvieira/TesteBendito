using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;



public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }

    // Relacionamento com Order

    [JsonIgnore]  // Isso vai evitar que a propriedade Order cause o ciclo.
    public Order Order { get; set; }

    // Relacionamento com Product
    public Product Product { get; set; }
}