﻿using GestionTarea.Data;
using GestionTarea.Models;
using GestionTarea.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestionTarea.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public int Id_Usuario { get; private set; }

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tarea> CrearTareaAsync(Tarea tarea)
        {
            Console.WriteLine($"Guardando tarea: {tarea.Titulo}, Usuario ID: {tarea.Id_Usuario}");

            _context.Tarea.Add(tarea);
            await _context.SaveChangesAsync();
            return tarea;
        }

        public async Task<bool> MarcarComoCompletadaAsync(int tareaId, int Id_Usuario)
        {
            var tarea = await _context.Tarea.FirstOrDefaultAsync(t => t.Id == tareaId && t.Id_Usuario == Id_Usuario);
            if (tarea == null) return false;

            tarea.Completada = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ActualizarTareaAsync(Tarea tarea)
        {
            var tareaExistente = await _context.Tarea.FindAsync(tarea.Id);
            if (tareaExistente == null)
                throw new Exception("La tarea no existe.");

            tareaExistente.Titulo = tarea.Titulo;
            tareaExistente.Descripcion = tarea.Descripcion;
            tareaExistente.FechaVencimiento = tarea.FechaVencimiento;
            tareaExistente.Completada = tarea.Completada;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> EliminarTareaAsync(int tareaId, int usuarioId)
        {
            var tarea = await _context.Tarea.FirstOrDefaultAsync(t => t.Id == tareaId && t.Id_Usuario == usuarioId);
            if (tarea == null) return false;

            tarea.Borrado = true; 
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Tarea>> ListarTareasAsync(bool? completada = null, DateTime? fechaVencimiento = null)
        {
            IQueryable<Tarea> query = _context.Tarea
                .Where(t => !t.Borrado) 
                .Include(t => t.Usuario); 

            if (completada.HasValue)
                query = query.Where(t => t.Completada == completada.Value);

            if (fechaVencimiento.HasValue)
                query = query.Where(t => t.FechaVencimiento <= fechaVencimiento.Value);

            return await query.ToListAsync();
        }


        public async Task<Usuario> ObtenerUsuarioPorGuidAsync(Guid userGuid)
        {
            return await _context.Usuario.FirstOrDefaultAsync(u => u.Id == userGuid.ToString());
        }

        public async Task<Tarea> ObtenerTareaPorIdAsync(int id)
        {
            return await _context.Tarea.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> EditarTareaAsync(Tarea tareaEditada, int usuarioId)
        {
            var tarea = await _context.Tarea.FirstOrDefaultAsync(t => t.Id == tareaEditada.Id && t.Id_Usuario == usuarioId);
            if (tarea == null)
                return false;

            tarea.Titulo = tareaEditada.Titulo;
            tarea.Descripcion = tareaEditada.Descripcion;
            tarea.FechaVencimiento = tareaEditada.FechaVencimiento;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EstadisticasTareasViewModel> ObtenerEstadisticasAsync(int usuarioId)
        {
            var totalTareasCreadas = await _context.Tarea.CountAsync();
            var tareasPendientes = await _context.Tarea.CountAsync(t => t.Id_Usuario == usuarioId && !t.Completada && !t.Borrado);
            var tareasCompletadas = await _context.Tarea.CountAsync(t => t.Id_Usuario == usuarioId && t.Completada && !t.Borrado);

            return new EstadisticasTareasViewModel
            {
                TotalTareasCreadas = totalTareasCreadas,
                TareasPendientes = tareasPendientes,
                TareasCompletadas = tareasCompletadas
            };
        }
    }
}
