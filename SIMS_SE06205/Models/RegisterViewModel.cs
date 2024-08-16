using System.ComponentModel.DataAnnotations;

namespace SIMS_SE06205.Models
{
    public class RegisterViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Username cannot be empty")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

       /* [Required(ErrorMessage = "Email cannot be empty")]*/
        /*[EmailAddress(ErrorMessage = "Invalid Email Address")]*/
       /* public string Email { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]*/
       /* public string PhoneNumber { get; set; }*/

        
    }
}
