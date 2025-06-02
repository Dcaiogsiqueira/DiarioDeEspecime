using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DiarioDeEspecime.Models;

namespace DiarioDeEspecime.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Especie> Especies { get; set; }
        public DbSet<Especime> Especimes { get; set; }
        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<UsuarioProjeto> UsuarioProjeto { get; set; }
        public DbSet<Bioma> Biomas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Important: call base to configure Identity tables
            base.OnModelCreating(modelBuilder);

            // (Optional) if you want to rename the default "AspNetUsers" table:
            // modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            // Composite key for UsuarioProjeto
            modelBuilder.Entity<UsuarioProjeto>()
                .HasKey(up => new { up.ProjetoId, up.UsuarioId });

            modelBuilder.Entity<UsuarioProjeto>()
                .HasOne(up => up.Projeto)
                .WithMany(p => p.UsuariosProjeto)
                .HasForeignKey(up => up.ProjetoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UsuarioProjeto>()
                .HasOne(up => up.Usuario)
                .WithMany(u => u.UsuariosProjeto)
                .HasForeignKey(up => up.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Especime → Criador
            modelBuilder.Entity<Especime>()
                .HasOne(e => e.Criador)
                .WithMany()
                .HasForeignKey(e => e.CriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Especime → Especie
            modelBuilder.Entity<Especime>()
                .HasOne(e => e.Especie)
                .WithMany(e => e.Especimes)
                .HasForeignKey(e => e.EspecieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
