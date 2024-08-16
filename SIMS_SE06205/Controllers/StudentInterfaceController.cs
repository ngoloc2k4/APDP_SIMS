using Microsoft.AspNetCore.Mvc;

namespace SIMS_SE06205.Controllers
{
    public class StudentInterfaceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
