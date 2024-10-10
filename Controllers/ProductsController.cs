using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bendito.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bendito.Controllers
{
    // Define a rota base para o contrller de produtos
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Permite o acesso as entidades do banco de dados
        private readonly ApplicationDbContext _context;

        //Receb o context do banco de dados
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método GET
        // A rota sera /api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // Consulta assíncrona para retornar todos os produtos e suas categorias associadas
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // Método GET by id
        // A rota será /api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // Busca assíncrona por um produto com base no ID e sua categoria associada
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            // Se o produto não for encontrado retorna uma resposta 404 Not Found
            if (product == null)
            {
                return NotFound();
            }

            // Retorna o produto encontrado.
            return product;
        }

        // Método PUT que atualiza um produto existente com base no ID fornecido
        // A rota será /api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            // Verifica se o ID fornecido na URL corresponde ao ID do produto fornecido
            // Se não corresponder retorna um erro 400 Bad Request
            if (id != product.Id)
            {
                return BadRequest();
            }

            // Marca o produto como modificado no banco de dados
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                // Tenta salvar as alterações no banco de dados de forma assíncrona
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se ocorrer uma exceção de concorrência verifica se o produto ainda existe no banco de dados
                if (!ProductExists(id))
                {
                    return NotFound(); // Se o produto não existir retorna 404 Not Found
                }
                else
                {
                    throw; // Caso contrário lança a exceção para tratamento posterior
                }
            }

            // Retorna uma resposta 204 No Content indicando que a atualização foi bem-sucedida.
            return NoContent();
        }

        // Método POST que cria um novo produto com base nos dados fornecidos.
        // A rota será /api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            // Busca a categoria associada ao produto com base no CategoryId fornecido
            var category = await _context.Categories.FindAsync(productDto.CategoryId);

            // Se a categoria não for encontrada retorna 404 Not Found com uma mensagem
            if (category == null)
            {
                return NotFound("Categoria não encontrada");
            }

            // Cria um novo objeto de produto com os dados fornecidos no DTO
            var product = new Product
            {
                Name = productDto.Name,                   
                Description = productDto.Description,     
                Price = productDto.Price,                 
                StockQuantity = productDto.StockQuantity, 
                CategoryId = productDto.CategoryId        
            };

            // Adiciona o novo produto ao context do banco de dados
            _context.Products.Add(product);
            // Salva as alterações no banco de dados de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna o produto criado incluindo o ID gerado e a rota gerada
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // Método DELETE que exclui um produto com base no ID fornecido
        // A rota será /api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Busca o produto com base no ID fornecido
            var product = await _context.Products.FindAsync(id);

            // Se o produto não for encontrado retorna 404 Not Found
            if (product == null)
            {
                return NotFound();
            }

            // Remove o produto do context do banco de dados
            _context.Products.Remove(product);
            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna uma resposta 204 No Content indicando que a exclusão foi bem sucedida.
            return NoContent();
        }

        // Método auxiliar privado que verifica se um produto com o ID fornecido existe no banco de dados
        private bool ProductExists(int id)
        {
            // Verifica se há algum produto no banco de dados com o ID fornecido
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
