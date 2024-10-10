using Microsoft.EntityFrameworkCore;  
using Microsoft.Extensions.DependencyInjection; 

var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço de DbContext configura o Entity Framework para usar o banco de dados 
// A string de conexão é recuperada do arquivo de configuração appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));  

// Configura os controllers permitindo que eles gerenciem requisições HTTP
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = null;  // Remove o gerenciador de referências circulares do JSON
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Adiciona o Swagger para testar a API


var app = builder.Build();

// Se estivera aplicação estiver em ambiente de desenvolvimento o Swagger é habilitado

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        
    app.UseSwaggerUI();      
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
