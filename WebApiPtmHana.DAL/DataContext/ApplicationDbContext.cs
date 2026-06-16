using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Lineas> Lineas { get; set; }
        public DbSet<EnvioSAP> EnvioSAP { get; set; }
        public DbSet<EstadosSAP> EstadosSap { get; set; }
        public DbSet<EnvioScrap> EnvioScraps { get; set; }
        public DbSet<TicketsInfo> TicketsInfos { get; set; }
        public DbSet<ScrapMolinos> ScrapMolinos { get; set; }
        public DbSet<HistorialPesadas> HistorialPesadas { get; set; }
        public DbSet<ProductoTerminado> ProductoTerminados { get; set; }
        public DbSet<AcumuladoEmbarque> AcumuladoEmbarques { get; set; }
        public DbSet<BitHistorialPesadas> BitHistorialPesadas { get; set; }
        public DbSet<EstatusHistorialPesadas> EstatusHistorialPesadas { get; set; }
        public DbSet<BitacoraAsignacionLinia> BitacoraAsignacionLinias { get; set; }
        public DbSet<UserTokens> UserTokens { get; set; }
        public DbSet<SkuInfo> SkuInfos { get; set; }
        public DbSet<EstadosPesos> EstadosPesos { get; set; }
        public DbSet<EstadosSemaforo> EstadosSemaforos { get; set; }
        public DbSet<BaseImpresion> BaseImpresion { get; set; }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../WebApiPtmHana/appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("SQLConnection");
            builder.UseSqlServer(connectionString);
            return new ApplicationDbContext(builder.Options);
        }
    }
}
