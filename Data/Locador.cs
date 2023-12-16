using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitAqui.Data
{
    public class Locador
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public bool EstadoSubscricao { get; set; }

        public int? AvaliacaoLocador { get; set; }  

        public List<RecursoHumano>? RecursoHumano { get; set; }


    }
}
