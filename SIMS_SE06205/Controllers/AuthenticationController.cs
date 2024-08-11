using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIMS_SE06205.Models;

namespace SIMS_SE06205.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly string _filePathUsers = @"F:\SIMS_practice\APDP-BTec-main\data-sims\users.json";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var users = await LoadUsersAsync();
            var user = users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                // Authentication successful
                // Set authentication cookie or session here
                if (user.Role == "admin")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.Role == "teacher")
                {
                    return RedirectToAction("TeacherInterface", "Home");
                }
                else if (user.Role == "student")
                {
                    return RedirectToAction("StudentInterface", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /* [HttpPost]
         public async Task<IActionResult> Register(RegisterViewModel model)
         {
             if (!ModelState.IsValid)
             {
                 return View(model);
             }

             var users = await LoadUsersAsync();
             if (users.Any(u => u.Email == model.Email))
             {
                 ModelState.AddModelError("", "Email already exists.");
                 return View(model);
             }

             var newUser = new UserViewModel
             {
                 Email = model.Email,
                 Password = model.Password, // Ensure this is hashed in a real application
                 Role = model.Role
             };

             users.Add(newUser);
             await SaveUsersAsync(users);

             return RedirectToAction("Index");
         }
 */
        public async Task<List<UserViewModel>> LoadUsersAsync()
        {
            if (!System.IO.File.Exists(_filePathUsers))
            {
                return new List<UserViewModel>();
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(_filePathUsers);
            return JsonConvert.DeserializeObject<List<UserViewModel>>(jsonData) ?? new List<UserViewModel>();
        }

        public async Task SaveUsersAsync(List<UserViewModel> users)
        {
            var jsonData = JsonConvert.SerializeObject(users, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(_filePathUsers, jsonData);
        }
    }
}
