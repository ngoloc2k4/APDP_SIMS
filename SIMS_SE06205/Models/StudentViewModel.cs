using System.ComponentModel.DataAnnotations;

namespace SIMS_SE06205.Models
{
    public class StudentModel
    {
        public List<StudentViewModel>? StudentsList { get; set; }
    }
    public class StudentViewModel
    {
        [Key]
        public string? Id { get; set; }
        [Required]
        public string? StudentCode { get; set; }

        [Required(ErrorMessage = "Student's Name can be not empty")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Student's Last Name can be not empty")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email can be not empty")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Your BirthDay can be not empty")]
        public string? DateOfBirth { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Address { get; set; }
    }
}
