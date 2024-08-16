using System.ComponentModel.DataAnnotations;

namespace SIMS_SE06205.Models
{
    public class CourseModel
    {
        public List<CourseViewModel> CourseLists { get; set; }
    }
    public class CourseViewModel
    {
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng điền thêm thông tin")]
        public string NameCourse { get; set; }

        public string? Description { get; set; }

    }
}
