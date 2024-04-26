using System.ComponentModel.DataAnnotations;

namespace MemoryboardAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MinLength(8)]
        [MaxLength(32)]
        public string Password { get; set; }
    }
}