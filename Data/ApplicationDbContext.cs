using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using HabitAqui.ViewModels;

namespace HabitAqui.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RecursoHumano> recursoshumanos { get; set; }

        public DbSet<Arrendamento> arrendamentos {  get; set; }

        public DbSet<CategoriaHabitacao> categoriaHabitacoes { get; set; }

        public DbSet<DanosHabitacao> danosHabitacaes { get; set; }

        public DbSet<EquipamentoOpcional> equipamentosOpcional { get; set;}

        public DbSet<Habitacao> habitacoes { get; set; }

        public DbSet<Locador> locadores { get; set; }

        public DbSet<Observacao> observacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
        }


    }
}