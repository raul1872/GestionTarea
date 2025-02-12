namespace GestionTarea.ViewModels
{
    public class TareaViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public bool Completada { get; set; }
    }

    public class ListaTareasViewModel
    {
        public List<TareaViewModel> Tareas { get; set; }
        public bool? FiltroCompletadas { get; set; }
        public DateTime? FiltroFechaVencimiento { get; set; }
    }
}
