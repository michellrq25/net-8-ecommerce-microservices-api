using Microsoft.EntityFrameworkCore;
using PF.Sol.EC.Pedidos.Domain.Entities;

namespace PF.Sol.EC.Pedidos.Infraestructure.Data
{
    public class PedidoDbContext : DbContext
    {
        public PedidoDbContext(DbContextOptions<PedidoDbContext> options) : base(options)
        {            
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(e =>
            {
                e.ToTable("Cliente");
                e.HasKey(x => x.IdCliente);
                e.Property(x => x.NombreCliente).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Pedido>(e =>
            {
                e.ToTable("Pedido");
                e.HasKey(x => x.IdPedido);
                e.Property(x => x.FechaPedido).IsRequired();
                e.Property(x => x.IdCliente).IsRequired();
                e.Property(x => x.MontoPedido).IsRequired().HasColumnType("decimal(9,2)");
                e.Property(x => x.FormaPago).IsRequired();
            });

            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { IdCliente = 1, NombreCliente = "Jon Snow" },
                new Cliente { IdCliente = 2, NombreCliente = "Daenerys Targaryen" },
                new Cliente { IdCliente = 3, NombreCliente = "Arya Stark" },
                new Cliente { IdCliente = 4, NombreCliente = "Tyrion Lannister" },
                new Cliente { IdCliente = 5, NombreCliente = "Cersei Lannister" }
                );
        }
    }
}
