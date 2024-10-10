using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// Definição do context do banco de dados que gerencia as entidades da aplicação
public class ApplicationDbContext : DbContext
{
    // Configuração do banco de dados 
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Definição das tabelas no banco de dados representadas pelas entidades
    public DbSet<Product> Products { get; set; }   
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Order> Orders { get; set; }       
    public DbSet<OrderItem> OrderItems { get; set; } 

    
    // Método que define como as tabelas no banco de dados estarão relacionadas.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Chama o método base para garantir que as configurações padrão do Entity Framework sejam aplicadas
        base.OnModelCreating(modelBuilder);

        // Configuração do relacionamento entre a entidade Order e OrderItem, um pedido pode ter vários itens 
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)            
            .WithOne(oi => oi.Order)               
            .HasForeignKey(oi => oi.OrderId);      

        // Configuração do relacionamento entre a entidade OrderItem e Product, um OrderItem está relacionado a um único produto
        // 
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)             
            .WithMany()                            
            .HasForeignKey(oi => oi.ProductId);    
    }
}
