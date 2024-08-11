using Microsoft.AspNetCore.Mvc;
using SIMS_SE06205.Models;
using Newtonsoft.Json;

namespace SIMS_SE06205.Controllers
{
    public class StudentController : Controller
    {
        private string filePathStudent = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-student.json";

        [HttpGet]
        public IActionResult Index()
        {
            string dataJson = System.IO.File.ReadAllText(filePathStudent);
            StudentModel model = new StudentModel();
            model.StudentsList = new List<StudentViewModel>();

            var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
            var dataStudent = (from s in students select s).ToList();
            foreach (var item in dataStudent)
            {
                model.StudentsList.Add(new StudentViewModel
                {
                    Id = item.Id,
                    StudentCode = item.StudentCode,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    DateOfBirth = item.DateOfBirth,
                    Gender = item.Gender,
                    PhoneNumber = item.PhoneNumber,
                    Address = item.Address,
                });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            StudentViewModel model = new StudentViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(StudentViewModel modelView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string dataJson = System.IO.File.ReadAllText(filePathStudent);
                    var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
                    int maxId = 0;
                    if (students != null)
                    {
                        maxId = int.Parse((from s in students select s.Id).Max()) + 1;
                    }
                    string idIncrement = maxId.ToString();
                    students.Add(new StudentViewModel
                    {
                        Id = idIncrement,
                        StudentCode = modelView.StudentCode,
                        FirstName = modelView.FirstName,
                        LastName = modelView.LastName,
                        Email = modelView.Email,
                        DateOfBirth = modelView.DateOfBirth,
                        Gender = modelView.Gender,
                        PhoneNumber = modelView.PhoneNumber,
                        Address = modelView.Address,
                    });
                    var dtJson = JsonConvert.SerializeObject(students, Formatting.Indented);
                    System.IO.File.WriteAllText(filePathStudent, dtJson);
                    TempData["saveStatus"] = true;
                }
                catch
                {
                    TempData["saveStatus"] = false;
                }
                return RedirectToAction(nameof(StudentController.Index), "Student");
            }
            return View(modelView);
        }

    }
}
