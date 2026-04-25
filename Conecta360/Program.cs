using Microsoft.EntityFrameworkCore;
using Conect360.Data;

var builder = WebApplication.CreateBuilder(args);

// ── ENTITY FRAMEWORK + SQL SERVER ──────────────────────────────────
builder.Services.AddDbContext<Conecta360DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Conecta360DB")));

// ── REPOSITORIO (Inyección de dependencias) ─────────────────────────
builder.Services.AddScoped<IContactoRepository, ContactoRepository>();

// ── MVC ──────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

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
