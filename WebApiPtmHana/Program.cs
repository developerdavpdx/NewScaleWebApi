using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.AcumuladoEmbServ;
using WebApiPtmHana.BLL.Services.BitacoraHpServ;
using WebApiPtmHana.BLL.Services.BitacoraLServ;
using WebApiPtmHana.BLL.Services.EnvioMolinosServ;
using WebApiPtmHana.BLL.Services.EnvioSapServ;
using WebApiPtmHana.BLL.Services.HistorialPesadasServ;
using WebApiPtmHana.BLL.Services.LineasServ;
using WebApiPtmHana.BLL.Services.ProductoTServ;
using WebApiPtmHana.BLL.Services.ScrapServ;
using WebApiPtmHana.BLL.Services.SkuInfoServ;
using WebApiPtmHana.BLL.Services.TarasServ;
using WebApiPtmHana.BLL.Services.TicketsInfoServ;
using WebApiPtmHana.BLL.Services.UserTokenServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.DAL.DataContext.Repositories.AcumuladoEmbRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.BitacoraHistorialPRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.BitacoraLiniaRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.EnvioMolinosRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.EnvioSapRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.HistorialPesadasRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.LineasRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.ProductoTRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.ScarpRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.SkuInfos;
using WebApiPtmHana.DAL.DataContext.Repositories.TarasRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.TicketsRepo;
using WebApiPtmHana.DAL.DataContext.Repositories.UserTokensRepo;
using WebApiPtmHana.Datos;
using Serilog;
using WebApiPtmHana.Shedule;
using WebApiPTMTest.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
var connectionStringSQL = builder.Configuration.GetConnectionString("SQLConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionStringSQL, b => b.MigrationsAssembly("WebApiPtmHana.DAL"));
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGenericRepositorieLinie<Lineas>, LineasRepositorie>();
builder.Services.AddScoped<IGenericRepoTickets<TicketsInfo>, TicketsRepositorie>();
builder.Services.AddScoped<IGenericRepoEnvioSap<EnvioSAP>, EnvioSapRepositorie>();
builder.Services.AddScoped<IGenericRepositorieScrap<ScrapMolinos>, ScrapRepositorie>();
builder.Services.AddScoped<IGenericRepoEnvioMolinos<EnvioScrap>, EnvioMolinosRepositorie>();
builder.Services.AddScoped<IGenericRepositorieProductoT<ProductoTerminado>, ProductoTRepo>();
builder.Services.AddScoped<IGenericRepoBitLinia<BitacoraAsignacionLinia>, BitAsignacionRepo>();
builder.Services.AddScoped<IGenericRepoAcumuladoEmb<AcumuladoEmbarque>, AcumuladoEmbarqueRepo>();
builder.Services.AddScoped<IGenericRepositorieHistPesada<BitHistorialPesadas>, HistPesadasRepositorie>();
builder.Services.AddScoped<IGenericRepoUserTokens<UserTokens>, UserTokensRepositorie>();
builder.Services.AddScoped<IGenericRepoSkuInfo<SkuInfo>, SkuInfoRepositorie>();
builder.Services.AddScoped<IGenericRepoHistorialPesadas<HistorialPesadas>, HistorialPesadasRepositorie>();
builder.Services.AddScoped<IGenericRepoTaras, TarasRepositorie>();

builder.Services.AddScoped<ILinia, LiniaService>();
builder.Services.AddScoped<IScrap, ScrapService>();
builder.Services.AddScoped<ISapEnvio, EnvioSapService>();
builder.Services.AddScoped<IInfoTickets, InfoTicketsService>();
builder.Services.AddScoped<IBitacoraLinia, BitacoraLService>();
builder.Services.AddScoped<IEnvioMolinos, EnvioMolinosService>();
builder.Services.AddScoped<IBitacoraHPesadas, BitacoraHpService>();
builder.Services.AddScoped<IProductoTerminado, ProductoTService>();
builder.Services.AddScoped<IAcumuladoEmbarque, AcumuladoEmbarqueService>();
builder.Services.AddScoped<IUserTokens, UserTokenService>();
builder.Services.AddScoped<ISkuInfo, SkuInfoService>();
builder.Services.AddScoped<IHistorialPesadas, HistorialPesadasService>();
builder.Services.AddScoped<ITaras, TarasService>();

//builder.Services.AddHostedService<SheduleTurno1>();
//builder.Services.AddHostedService<SheduleTurno2>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaulConnection");
builder.Services.AddSingleton(new HanaConnection(connectionString));

var app = builder.Build();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//JobManager.Initialize(new SheduleTurno1());

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

Log.Information("API iniciada correctamente");

app.Run();
