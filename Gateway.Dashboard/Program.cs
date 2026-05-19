using Gateway.Dashboard.Components;
using Gateway.Shared.Contexto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configura o banco de dados do Supabase Postgres
var conexaoPostgres = builder.Configuration.GetConnectionString("SupabasePostgres");
builder.Services.AddDbContextFactory<GatewayDbContext>(opcoes =>
    opcoes.UseNpgsql(conexaoPostgres));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
