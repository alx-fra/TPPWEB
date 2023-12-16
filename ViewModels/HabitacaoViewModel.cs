using System.ComponentModel.DataAnnotations.Schema;

namespace HabitAqui.ViewModels
{
    public class HabitacaoViewModel
    {
        public int Id { get; set; }

        public string Localizacao { get; set; }

        public string TipoHabitacao { get; set; }

        public float CustoArrendamento { get; set; }

        public int AvaliacaoLocador { get; set; }

        public string Estado { get; set; }

        public int CategoriaHabitacaoId { get; set; }

        public string NomeLocador { get; set; }

        public string? ImagemUrl { get; set; }

        [NotMapped]
        public IFormFile? ImagemFile { get; set; }
    }

}
