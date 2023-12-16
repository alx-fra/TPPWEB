using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace HabitAqui.Data
{
    public class RecursoHumano
    {
        public int Id { get; set; }

        public string IdRecHum { get; set; }
        public string RoleNoLocador { get; set; }

        [ForeignKey("Locador")]
        public int LocadorId { get; set; }
        public Locador Locador { get; set; }
    }
}
