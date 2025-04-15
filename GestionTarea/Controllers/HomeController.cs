using System.Diagnostics;
using System.Security.Claims;
using GestionTarea.Data;
using GestionTarea.Models;
using GestionTarea.Services;
using GestionTarea.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionTarea.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITaskService _taskService;
        private readonly AppDbContext _context;


        public HomeController(ILogger<HomeController> logger, ITaskService taskService, AppDbContext context)
        {
            _logger = logger;
            _taskService = taskService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                int usuarioId = await ObtenerIdUsuario();
                var estadisticas = await _taskService.ObtenerEstadisticasAsync(usuarioId);
                return View(estadisticas);
            }

            return View(null); 
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Tarea(bool? completadas, DateTime? fechaVencimiento)
        {
            Guid userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            int usuarioId = await _context.Usuario
                .Where(u => u.Id == userGuid.ToString())
                .Select(u => u.Id_Usuario)
                .FirstOrDefaultAsync();

            if (usuarioId == 0)
            {
                return Unauthorized(); 
            }

            var tareas = await _taskService.ListarTareasAsync(completadas, fechaVencimiento);

            var model = new ListaTareasViewModel
            {
                Tareas = tareas.Select(t => new TareaViewModel
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    FechaVencimiento = t.FechaVencimiento,
                    Completada = t.Completada,
                    Id_UsuarioAsignado = t.Id_Usuario,
                    NombreUsuarioAsignado = t.Usuario?.Nombre ?? "Desconocido"
                }).ToList(),
                FiltroCompletadas = completadas,
                FiltroFechaVencimiento = fechaVencimiento
            };

            return View(model);

        }

        //-----------------------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarios = await _context.Usuario
                .Select(u => new UsuarioViewModel
                {
                    Id_Usuario = u.Id_Usuario,
                    Nombre = u.Nombre 
                })
                .ToListAsync();

            var model = new TareaViewModel
            {
                UsuariosDisponibles = usuarios
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TareaViewModel model)
        {
            _logger.LogInformation("Se ha recibido una solicitud para crear una nueva tarea");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo no es válido");

                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        _logger.LogError($"Error en {error.Key}: {subError.ErrorMessage}");
                    }
                }

                model.UsuariosDisponibles = await _context.Usuario
                    .Select(u => new UsuarioViewModel
                    {
                        Id_Usuario = u.Id_Usuario,
                        Nombre = u.Nombre
                    })
                    .ToListAsync();

                return View(model);
            }

            int usuarioId = await ObtenerIdUsuario();
            _logger.LogInformation($"ID del usuario autenticado: {usuarioId}");

            var nuevaTarea = new Tarea
            {
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                FechaVencimiento = model.FechaVencimiento,
                Completada = false,
                Id_Usuario = model.Id_UsuarioAsignado ?? usuarioId
            };

            _logger.LogInformation($"Guardando tarea: {nuevaTarea.Titulo}");
            await _taskService.CrearTareaAsync(nuevaTarea);

            return RedirectToAction("Tarea");
        }

        //-------------------------------------------------------------------------
        private async Task<int> ObtenerIdUsuario()
        {
            var userGuidString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userGuidString))
            {
                throw new Exception("No se pudo obtener el ID del usuario.");
            }

            if (!Guid.TryParse(userGuidString, out Guid userGuid))
            {
                throw new Exception("El ID del usuario no es un GUID válido.");
            }

            var usuario = await _taskService.ObtenerUsuarioPorGuidAsync(userGuid);

            if (usuario == null)
            {
                throw new Exception("Usuario no encontrado en la base de datos.");
            }

            return usuario.Id_Usuario;
        }

        //--------------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> MarcarCompletada(int tareaId)
        {
            int usuarioId = await ObtenerIdUsuario();
            await _taskService.MarcarComoCompletadaAsync(tareaId, usuarioId);
            return RedirectToAction("Tarea");
        }

        //---------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var tarea = await _taskService.ObtenerTareaPorIdAsync(id);
            if (tarea == null)
                return NotFound();

            var model = new TareaViewModel
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaVencimiento = tarea.FechaVencimiento,
                Completada = tarea.Completada
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TareaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userGuid = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var usuario = await _taskService.ObtenerUsuarioPorGuidAsync(userGuid);
            if (usuario == null)
                return BadRequest("Usuario no encontrado.");

            var tareaExistente = await _taskService.ObtenerTareaPorIdAsync(model.Id);
            if (tareaExistente == null)
                return NotFound("Tarea no encontrada.");

            if(tareaExistente.Completada)
            {
                ModelState.AddModelError("Completada", "No puedes editar una tarea que ya está completa.");
                return View(model);  
            }
            var tarea = new Tarea
            {
                Id = model.Id,
                Titulo = model.Titulo,
                Descripcion = model.Descripcion,
                FechaVencimiento = model.FechaVencimiento,
                Completada = model.Completada,
                Id_Usuario = usuario.Id_Usuario
            };

            await _taskService.ActualizarTareaAsync(tarea);
            return RedirectToAction("Tarea");
        }

        //--------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Eliminar(int tareaId)
        {
            int usuarioId = await ObtenerIdUsuario();
            await _taskService.EliminarTareaAsync(tareaId, usuarioId);
            return RedirectToAction("Tarea");
        }

        


    }
}



