using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }

    // Relacionamento: um pedido tem vários itens
    public ICollection<OrderItem> OrderItems { get; set; }
}