using Microsoft.AspNetCore.Mvc;

namespace SIMS_SE06205.Controllers
{
    public class TeacherInterfaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
