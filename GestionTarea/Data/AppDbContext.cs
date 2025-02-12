using GestionTarea.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionTarea.Data
{
    public class AppDbContext : IdentityDbContext<Usuario>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Tarea> Tarea { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasSequence<int>("Seq_Usuario_Id")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id_Usuario)
                .HasDefaultValueSql("NEXT VALUE FOR Seq_Usuario_Id")
                .IsRequired();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Id_Usuario)
                .IsUnique();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Id_Usuario)
                .IsUnique();

            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tarea)
                .HasForeignKey(t => t.Id_Usuario)
                .HasPrincipalKey(u => u.Id_Usuario);
        }
    }
}
