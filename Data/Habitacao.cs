using System.ComponentModel.DataAnnotations.Schema;

namespace HabitAqui.Data
{
    public class Habitacao
    {
        public int Id { get; set; }

        public string Localizacao { get; set; }

        public string TipoHabitacao { get; set; }

        public float CustoArrendamento { get; set; }

        public string Estado { get; set; }

        public int CategoriaHabitacaoId { get; set; }
        public int LocadorId { get; set; }
        public string? ImagemUrl { get; set; } 

        [NotMapped] 
        public IFormFile? ImagemFile { get; set; }

        [NotMapped]
        public virtual Locador? Locador { get; set; }
    }
}
