using Microsoft.AspNetCore.Mvc;

namespace SIMS_SE06205.Controllers
{
    public class UserManagementController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        

    }
}
