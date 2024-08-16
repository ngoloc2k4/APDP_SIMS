namespace SIMS_SE06205.Models
{
    public class UserManagementModel
    {
        public List<UserManagementViewModel> UserManagement { get; set; }
    }

    public class UserManagementViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string CodeID { get; set; }
    }
}
