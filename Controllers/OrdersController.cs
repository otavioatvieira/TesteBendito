using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bendito.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bendito.Controllers
{
    // Define a rota base para o controller de Orders
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // Permite o acesso as entidades do banco de dados
        private readonly ApplicationDbContext _context;

        // Recebe o context do banco de dados
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método GET 
        // A rota será /api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // Retorna todos os pedidos incluindo os itens do pedido e os produtos com suas respectivas categorias
            return await _context.Orders
                .Include(o => o.OrderItems)              
                .ThenInclude(oi => oi.Product)             
                .ThenInclude(p => p.Category)              
                .ToListAsync();                            
        }

        // Método GET by id
        // A rota será /api/Orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            // Busca um pedido específico com base no ID
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category) 
                .FirstOrDefaultAsync(o => o.Id == id); // Retorna o primeiro resultado correspondente ao ID ou null se não houver

            // Se o pedido não for encontrado retorna uma resposta 404 Not Found
            if (order == null)
            {
                return NotFound();
            }

            // Retorna o pedido encontrado.
            return order;
        }

        // Método PUT que atualiza um pedido existente
        // A rota será /api/Orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            // Verifica se o ID passado na URL corresponde ao ID do pedido fornecido
            // Se não corresponder retorna um erro 400 Bad Request
            if (id != order.Id)
            {
                return BadRequest();
            }

            // Marca o pedido como modificado no context do banco de dados
            _context.Entry(order).State = EntityState.Modified;

            try
            {
                // Tenta salvar as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            // Se ocorrer um erro de concorrência (ou seja, outra operação modificou o pedido ao mesmo tempo) captura a exceção
            catch (DbUpdateConcurrencyException)
            {
                // Verifica se o pedido ainda existe no banco de dados
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Repassa a exceção se for outro tipo de erro
                }
            }

            // Retorna uma resposta 204 No Content indicando que a operação foi bem sucedida mas sem conteúdo a ser retornado.
            return NoContent();
        }

        // Método POST que cria um novo pedido.
        // A rota será /api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            // Converter a data do pedido para UTC
            var orderDateUtc = DateTime.SpecifyKind(orderDto.OrderDate, DateTimeKind.Utc);

            // Criar o objeto Order com os dados fornecidos
            var order = new Order
            {
                UserId = orderDto.UserId,            
                OrderDate = orderDateUtc,            
                TotalAmount = orderDto.TotalAmount,  
                OrderItems = new List<OrderItem>()   
            };

            // Processa cada item do pedido fornecido no DTO
            foreach (var item in orderDto.OrderItems)
            {
                // Busca o produto com base no ID fornecido no item do pedido
                var product = await _context.Products
                    .Include(p => p.Category)         
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId); // Retorna o primeiro produto correspondente ao ID ou null

                // Se o produto não for encontrado retorna uma resposta 404 Not Found com mensagem específica
                if (product == null)
                {
                    return NotFound($"Produto com ID {item.ProductId} não encontrado");
                }

                // Cria o item do pedido associando o produto, quantidade e calculando o preço total (cálculo falhando)
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity, 
                    Product = product,                        
                    Order = order                             
                };

                // Adiciona o item a lista de itens do pedido
                order.OrderItems.Add(orderItem);
            }

            // Adiciona o novo pedido ao context do banco de dados
            _context.Orders.Add(order);
            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna o pedido criado incluindo o ID gerado e a rota criada
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // Método DELETE que exclui um pedido com base no ID fornecido
        // A rota será /api/Orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Busca o pedido com base no ID fornecido
            var order = await _context.Orders.FindAsync(id);

            // Se o pedido não for encontrado retorna uma resposta 404 Not Found
            if (order == null)
            {
                return NotFound();
            }

            // Remove o pedido do context do banco de dados
            _context.Orders.Remove(order);
            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna uma resposta 204 No Content indicando que a exclusão foi bemsucedida
            return NoContent();
        }

        // Método auxiliar privado que verifica se um pedido com o ID fornecido existe no banco de dados
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
