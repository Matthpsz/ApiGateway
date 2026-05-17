using Gateway.Shared.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Shared.Contexto;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> opcoes) : base(opcoes)
    {
    }

    public DbSet<LogTelemetria> LogsTelemetria { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Indexar por Data e Rota melhora absurdamente a performance dos gráficos no Dashboard
        modelBuilder.Entity<LogTelemetria>()
            .HasIndex(l => l.DataRequisicao);
            
        modelBuilder.Entity<LogTelemetria>()
            .HasIndex(l => l.Rota);
    }
}