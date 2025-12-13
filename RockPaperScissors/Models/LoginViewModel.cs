using System.ComponentModel.DataAnnotations;

namespace RockPaperScissors.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Név")]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; } = null!;
    }
}
