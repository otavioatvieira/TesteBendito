using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bendito.Controllers
{
    // Define a rota base para o controller de categories
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        // Permite o acesso as entidades do banco de dados
        private readonly ApplicationDbContext _context;

        // Recebe o context do banco de dados
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método GET 
        // A rota será /api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            // Busca todas as categorias de forma assíncrona e as retorna em uma lista
            return await _context.Categories.ToListAsync();
        }

        // Método GET by id
        // A rota será /api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            // Busca a categoria com o ID fornecido no banco de dados
            var category = await _context.Categories.FindAsync(id);

            // Se a categoria não for encontrada retorna uma resposta 404 Not Found
            if (category == null)
            {
                return NotFound();
            }

            // Se a categoria for encontrada retorna a categoria
            return category;
        }

        // Método PUT que atualiza uma categoria existente
        // A rota será /api/Categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            // Verifica se o ID passado na URL corresponde ao ID da categoria fornecida.
            // Se não corresponder retorna um erro 400 Bad Request
            if (id != category.Id)
            {
                return BadRequest();
            }

            // Marca a categoria como modificada no context do banco de dados
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                // Tenta salvar as alterações no banco de dados
                await _context.SaveChangesAsync();
            }
            // Se ocorrer um erro de concorrência captura a exceção.
            catch (DbUpdateConcurrencyException)
            {
                // Verifica se a categoria ainda existe no banco de dados
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Repassa a exceção se for outro tipo de erro
                }
            }

            // Retorna uma resposta 204 No Content indicando que a operação foi bem-sucedida mas sem conteúdo a ser retornado
            return NoContent();
        }

        // Método POST que cria uma nova categoria
        // A rota será /api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            // Adiciona a nova categoria ao context do banco de dados
            _context.Categories.Add(category);
            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna uma resposta 201 Created com o caminho da nova categoria e seus dados
            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // Método DELETE que exclui uma categoria com base no ID fornecido.
        // A rota será /api/Categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Busca a categoria com o ID fornecido no banco de dados
            var category = await _context.Categories.FindAsync(id);

            // Se a categoria não for encontrada retorna uma resposta 404 Not Found
            if (category == null)
            {
                return NotFound();
            }

            // Remove a categoria do context do banco de dados
            _context.Categories.Remove(category);
            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Retorna uma resposta 204 No Content indicando que a exclusão foi bem sucedida
            return NoContent();
        }

        // Método auxiliar privado que verifica se uma categoria com o id fornecido existe no banco de dados
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
