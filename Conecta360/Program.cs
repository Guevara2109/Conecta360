using Conect360.Data;
using Conect360.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── ENTITY FRAMEWORK + SQL SERVER ──────────────────────────────────
builder.Services.AddDbContext<Conecta360DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Conecta360DB")));

// ── REPOSITORIO (Inyección de dependencias) ─────────────────────────
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();

// Agrege el servicio de ContactoService a la inyección de dependencias
builder.Services.AddScoped<IContactoService, ContactoService>();

// ── MVC ──────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
