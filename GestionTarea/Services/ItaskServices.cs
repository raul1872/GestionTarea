using GestionTarea.Models;
using GestionTarea.ViewModels;

namespace GestionTarea.Services
{
    public interface ITaskService
    {
        Task<Tarea> CrearTareaAsync(Tarea tarea);
        Task<bool> MarcarComoCompletadaAsync(int tareaId, int usuarioId);
        Task<bool> EditarTareaAsync(Tarea tareaEditada, int usuarioId);
        Task<bool> EliminarTareaAsync(int tareaId, int usuarioId);
        Task<List<Tarea>> ListarTareasAsync(int usuarioId, bool? completada = null, DateTime? fechaVencimiento = null);
        Task<Usuario> ObtenerUsuarioPorGuidAsync(Guid userGuid);
        Task<Tarea> ObtenerTareaPorIdAsync(int id);
        Task ActualizarTareaAsync(Tarea tarea);
    }
}
