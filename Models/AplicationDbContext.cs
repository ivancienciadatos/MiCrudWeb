using Microsoft.EntityFrameworkCore; // Herramienta oficial de Microsoft para bases de datos

namespace MiCrudWeb.Models
{
    // Esta clase hereda de DbContext para la conexión
    public class AplicationDbContext : DbContext
    {
        // El constructor recibe la configuración (como la dirección del servidor) y se la pasa al motor base
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options)
        {
        }

        // Esta propiedad representa la tabla real dentro de SQL Server.
        // Le dice a .NET: "Mapea mi clase 'Persona' con la tabla 'Personas' de la base de datos".
        public DbSet<Persona> Personas { get; set; }
    }
}
