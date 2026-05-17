using Gateway.Shared.Contexto;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configura o SQL Server apontando para o projeto Shared
var conexaoPostgres = builder.Configuration.GetConnectionString("SupabasePostgres");
builder.Services.AddDbContext<GatewayDbContext>(opcoes =>
    opcoes.UseNpgsql(conexaoPostgres, b => b.MigrationsAssembly("Gateway.API")));
    
// Configura o Redis
var conexaoRedis = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(conexaoRedis));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();


app.UseMiddleware<FiltroTelemetria>();       // 1º Abre o cronômetro para medir tudo
app.UseMiddleware<FiltroLimiteRequisicoes>(); // 2º Valida o Rate Limiting. Se bloquear, o filtro 1 captura o erro 429

app.MapReverseProxy(); // 3º Se passar pelos dois, envia para a API final

app.Run();