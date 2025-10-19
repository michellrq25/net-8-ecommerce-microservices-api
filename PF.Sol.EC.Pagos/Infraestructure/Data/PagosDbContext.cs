using Microsoft.EntityFrameworkCore;
using PF.Sol.EC.Pagos.Domain.Entities;
using System.Diagnostics.Metrics;

namespace PF.Sol.EC.Pagos.Infraestructure.Data
{
    public class PagosDbContext : DbContext
    {

        public PagosDbContext(DbContextOptions<PagosDbContext> options) : base(options)
        {
        }

        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Pago Entity
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("Pago");
                entity.HasKey(e => e.IdPago);
                entity.Property(e => e.FechaPago).IsRequired();
                entity.Property(e => e.IdCliente).IsRequired();
                entity.Property(e => e.MontoPago).HasColumnType("decimal(9,2)").IsRequired();
                entity.Property(e => e.FormaPago).IsRequired();
            });

        }

    }
}
