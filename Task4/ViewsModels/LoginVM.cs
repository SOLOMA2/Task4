using System.ComponentModel.DataAnnotations;

namespace Task4.ViewsModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!; 

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!; 

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
