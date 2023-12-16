using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace HabitAqui.Data
{
    public class Arrendamento
    {
        public int Id { get; set; }

        public DateTime DataInicioContrato { get; set; }

        public DateTime DataFimContrato { get; set; }

        public float CustoArrendamento {  get; set; }

        public string estado {  get; set; }

        public int Avaliacao { get; set; }

        public int HabitacaoId { get; set; }

        [ForeignKey("HabitacaoId")]
        public Habitacao? Habitacao { get; set; }

        public EquipamentoOpcional? EquipamentoOpcional { get; set; }

        public Observacao? Observacoes { get; set;}

        public int PeriodoMinimo { get; set; }
        public DanosHabitacao? DanosHabitacao { get; set;}

        [ForeignKey("AspNetUser")]
        public string UserId { get; set; }
        public IdentityUser? AspNetUser { get; set; }

    }
}
