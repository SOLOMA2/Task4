using System.ComponentModel.DataAnnotations;

namespace Task4.ViewsModels
{
    public class RegisterVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!; 

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!; 

        [Compare("Password", ErrorMessage = "Password don't match")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!; 
    }
}
