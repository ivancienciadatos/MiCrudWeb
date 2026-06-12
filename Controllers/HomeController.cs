using Microsoft.AspNetCore.Mvc;
using MiCrudWeb.Models;

namespace MiCrudWeb.Controllers
{
    public class HomeController : Controller
    {
        // Crear una variable interna para guardar el acceso 
        private readonly AplicationDbContext _contexto;

        // El constructor recibe  automáticamente gracias al motor de .NET
        public HomeController(AplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // Esta es la función principal que responde  a la raíz de la web
        public IActionResult Index()
        {
            // Verifir conexión real con el motor de SQL Server
            if (_contexto.Database.CanConnect())
            {
                // Guardamos el mensaje de éxito en s 'ViewData'
                ViewData["MensajeConexion"] = "¡Conexión Exitosa a SQL Server desde el Navegador Web! ";
                ViewData["ColorMensaje"] = "alert-success"; // Código de color verde de internet
            }
            else
            {
                ViewData["MensajeConexion"] = " Error de conexión: No se pudo conectar a la base de datos.";
                ViewData["ColorMensaje"] = "alert-danger"; // Código de color rojo de internet
            }

            // de la tabla 'Personas' en SQL Server y las empaqueta en una lista de C#
            List<Persona> listaDeBaseDatos = _contexto.Personas.ToList();

            //  Contamos cuántas filas llegaron en la lista
            int totalAlumnos = listaDeBaseDatos.Count;

            // Guardamos el total 
            ViewData["TotalRegistrados"] = $"Total de personas en el sistema: {totalAlumnos}";

            return View(listaDeBaseDatos); // Le entrega el control a la página visual (la Vista)
        }


        // Cuando el usuario entre a la dirección /Home/Crear, se activará esto
        public IActionResult Crear()
        {
            return View(); // Abre la página visual vacía para que el usuario escriba
        }

        // ====================================================
        //RECIBIR LOS DATA Y MANDARLOS AL TÚNEL
        // ====================================================
        [HttpPost] // indica que esta función recibe datos de un formulario
        public IActionResult Crear(Persona nuevaPersona)
        {
            // Verificar  Revisa si el modelo cumple con las reglas (ej: que no esté vacío)
            if (ModelState.IsValid)
            {

                // Existe el RUN 
                bool yaExisteElRun = _contexto.Personas.Any(p => p.RUN == nuevaPersona.RUN);

                if (yaExisteElRun == true)
                {
                    // Si ya existe, alerta roja de validación 
                    ModelState.AddModelError("RUN", "¡Error! Este RUN ya se encuentra registrado BD");

                    // Volvemos a mostrar la misma página. .
                    return View(nuevaPersona);
                }






                // METER AL EMBUDO: Añadimos el objeto a nuestra compuerta del túnel
                _contexto.Personas.Add(nuevaPersona);

                // MANDAR POR LA TUBERÍA: Esta línea empuja físicamente los cambios a SQL Server
                _contexto.SaveChanges();

                // en la misma pantalla de inmediato
                ViewData["MensajeExito"] = $"¡Persona con RUN {nuevaPersona.RUN} registrada exitosamente! ";

                // Limpiar las casillas del modelo para que la siguiente vista nazca en blanco
                ModelState.Clear();

                return View(); // Se queda en el formulario 
            }


            // dRAMAS el formulario pintando las alertas rojas automáticas
            return View(nuevaPersona);
        }



        // ====================================================
        // UPDATE: BUSCAR LA PERSONA Y MOSTRAR SU FORMULARIO
        // ====================================================
        // Al hacer clic en Editar en la tabla, la web viaja acá trayendo el RUN en la variable 'id'
        public IActionResult Editar(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // Error de seguridad si el RUN viene vacío
            }

            // Buscamos en SQL Server 
            Persona personaEncontrada = _contexto.Personas.Find(id);

            if (personaEncontrada == null)
            {
                return NotFound(); // Error si el RUN no existe en el disco duro
            }

            // Le entregamos la persona encontrada al View(). 

            return View(personaEncontrada);
        }


        // ====================================================
        //  UPDATE: RECIBIR LOS CAMBIOS Y ACTUALIZAR EN BD
        // ====================================================
        [HttpPost]
        public IActionResult Editar(Persona personaModificada)
        {
            // Verificar  validación ( edad válida
            if (ModelState.IsValid)
            {
                // El comando '.Update()' busca automáticamente en SQL Server 
                _contexto.Personas.Update(personaModificada);

                // Confirmamos los cambios hacia el disco duro
                _contexto.SaveChanges();

                // Guardar un aviso de éxito para la pantalla principal
                TempData["MensajeAccion"] = $"¡Datos de '{personaModificada.Nombre}' actualizados con éxito! ";

                // Devolver al usuario al inicio
                return RedirectToAction("Index");
            }

            // erróneo al editar, recarga el formulario mostrando las alertas
            return View(personaModificada);
        }


        // ====================================================
        //  DELETE: BUSCAR LA PERSONA Y MOSTRAR CONFIRMACIÓN
        // ====================================================
        public IActionResult Eliminar(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Buscar el registro  usando el RUN (id)
            Persona personaEncontrada = _contexto.Personas.Find(id);

            if (personaEncontrada == null)
            {
                return NotFound();
            }

            //  vista para que el usuario vea a quién va a borrar
            return View(personaEncontrada);
        }

        // ====================================================
        // DELETE: ACCIÓN DE BORRADO FÍSICO EN SQL
        // ====================================================
        [HttpPost]
        // Usamos un nombre de acción personalizado para que no choque con la función anterior
        [ActionName("EliminarConfirmado")]
        public IActionResult EliminarConfirmado(Persona personaABorrar)
        {
            // .Remove() busca el registro por su RUN BD
            _contexto.Personas.Remove(personaABorrar);

            // Guardar los cambios permanentemente bd
            _contexto.SaveChanges();

            // Mensaje de notificación para el index
            TempData["MensajeAccion"] = "El registro ha sido eliminado correctamente de SQL Server. ";

            // Redireccionamos al inicio
            return RedirectToAction("Index");
        }







    }
}
