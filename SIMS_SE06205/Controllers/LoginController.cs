using Microsoft.AspNetCore.Mvc;
using SIMS_SE06205.Models;

namespace SIMS_SE06205.Controllers
{
    public class LoginController : Controller
    {
        private string filePathDataUser = @"F:\SIMS_practice\APDP-BTec-main\data-sims\users.json";

        [HttpGet]
        public IActionResult Index()
        {
            LoginViewModel vm = new LoginViewModel();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // lay thong tin tu form
                string username = model.UserName.Trim();
                string password = model.Password.Trim();

                // lấy data từ json file
                string dataJson = System.IO.File.ReadAllText(filePathDataUser);

                // kiem tra username va password co ton tai trong dataJson hay khong ?
                var people = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LoginViewModel>>(dataJson);
                var user = (from p in people
                            where p.UserName == username && p.Password == password
                            select p).FirstOrDefault();
                if (user == null)
                {
                    TempData["loginStatus"] = false;
                }
                else
                {
                    TempData["loginStatus"] = true;
                    // luu thong tin nguoi dung vao session
                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionUserId")))
                    {
                        HttpContext.Session.SetString("SessionUserId", user.Id);
                        HttpContext.Session.SetString("SessionUsername", user.UserName);
                        HttpContext.Session.SetString("SessionRole", user.Role);
                    }
                    // chuyen huong den trang tuong ung voi role
                    switch (user.Role.ToLower())
                    {
                        // chuyen huong den trang admin
                        case "admin":
                            return RedirectToAction(nameof(HomeController.Index), "Home");
                        // chuyen huong den trang teacher
                        case "teacher":
                            return RedirectToAction("Index", "TeacherInterface");
                        // chuyen huong den trang student
                        case "student":
                            return RedirectToAction("Index", "StudentInterface");
                        // mac dinh loi, quay ve trang login
                        default:
                            ModelState.AddModelError("", "Invalid role.");
                            return View(model);
                    }

                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // xoa session da tao ra o login
            // quay ve trang dang nhap
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SessionUserId")))
            {
                // xoa session
                HttpContext.Session.Remove("SessionUserId");
                HttpContext.Session.Remove("SessionUsername");
                HttpContext.Session.Remove("SessionRole");
            }
            // quay ve trang dang nhap
            return RedirectToAction(nameof(LoginController.Index), "Login");
        }
    }
}
