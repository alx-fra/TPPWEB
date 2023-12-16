using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HabitAqui.ViewModels
{
    public class CreateFuncViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public int LocadorId { get; set; }

    }
}
