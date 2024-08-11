using System.ComponentModel.DataAnnotations;

namespace SIMS_SE06205.Models
{
    public class AuthenticationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role { get; }
    }
}
