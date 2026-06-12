using Microsoft.EntityFrameworkCore;
using MiCrudWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// la conexión que esta en el archivo json
var conexionString = builder.Configuration.GetConnectionString("ConexionSQL");

// Registrar el túnel (AplicationDbContext) en el sistema operativo del sitio web
builder.Services.AddDbContext<AplicationDbContext>(options =>
    options.UseSqlServer(conexionString)
);

// Agregar servicios estándar de MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ====================================================
// CONEXIÓN REAL
// ====================================================

using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    try
    {
        var contexto = servicios.GetRequiredService<AplicationDbContext>();
        
        // El comando 'CanConnect()' intenta mandar un pulso eléctrico secreto a SQL Server
        if (contexto.Database.CanConnect())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n==================================================");
            Console.WriteLine(" CONFIRMACIÓN: ¡Conexión exitosa a SQL Server!");
            Console.WriteLine("==================================================\n");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n==================================================");
        Console.WriteLine($"  ERROR DE CONEXIÓN: {ex.Message}");
        Console.WriteLine("==================================================\n");
        Console.ResetColor();
    }
}

// Configuración básica del sitio web
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
