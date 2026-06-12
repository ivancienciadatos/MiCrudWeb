using System;
using System.ComponentModel.DataAnnotations; // herramientas activa las etiquetas de base de datos

namespace MiCrudWeb.Models
{
    // Esta clase representa la estructura de la tabla "Personas"
    public class Persona
    {
        [Key] // El RUN es la Clave Primaria obligatoria e irrepetible
        public string RUN { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")] // la casilla de internet nunca quede vacía
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(0, 120, ErrorMessage = "Ingrese una edad realista entre 0 y 120 años")] // Validación 
        public int Edad { get; set; }
    }
}
