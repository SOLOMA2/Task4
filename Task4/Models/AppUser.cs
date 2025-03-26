using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Task4.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginTime { get; set; } = DateTime.UtcNow;
    }
}
