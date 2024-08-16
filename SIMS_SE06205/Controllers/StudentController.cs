using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIMS_SE06205.Models;

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
            StudentViewModel student = new StudentViewModel();
            return View(student);
        }

        [HttpPost]
        public IActionResult Add(StudentViewModel StudentModelView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Read the JSON data from the file
                    string dataJson = System.IO.File.ReadAllText(filePathStudent);
                    var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson) ?? new List<StudentViewModel>();

                    // Check if StudentCode already exists
                    var existingStudent = students.FirstOrDefault(s => s.StudentCode == StudentModelView.StudentCode);
                    if (existingStudent != null)
                    {
                        ModelState.AddModelError("StudentCode", "StudentCode already exists.");
                        TempData["saveStatus"] = false;
                        return View(StudentModelView); // Return view with error
                    }

                    // Calculate the new ID
                    int maxId = 0;
                    if (students != null && students.Any())
                    {
                        maxId = students.Max(s => int.Parse(s.Id));
                    }
                    int newId = maxId + 1;

                    string idIncrement = newId.ToString();
                    students.Add(new StudentViewModel
                    {
                        Id = idIncrement,
                        StudentCode = StudentModelView.StudentCode,
                        FirstName = StudentModelView.FirstName,
                        LastName = StudentModelView.LastName,
                        Gender = StudentModelView.Gender,
                        Email = StudentModelView.Email,
                        DateOfBirth = StudentModelView.DateOfBirth,
                        PhoneNumber = StudentModelView.PhoneNumber,
                        Address = StudentModelView.Address,
                    });

                    var dtJson = JsonConvert.SerializeObject(students, Formatting.Indented);
                    System.IO.File.WriteAllText(filePathStudent, dtJson);
                    TempData["saveStatus"] = true;

                    return RedirectToAction(nameof(StudentController.Index), "Student");
                }
            }
            catch
            {
                TempData["saveStatus"] = false;
            }
            return View(StudentModelView);
        }


        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            try
            {
                string dataJson = System.IO.File.ReadAllText(filePathStudent);
                var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
                var student = students.Find(s => s.Id == id.ToString());
                if (student != null)
                {
                    students.Remove(student);
                    string updatedJson = JsonConvert.SerializeObject(students, Formatting.Indented);
                    System.IO.File.WriteAllText(filePathStudent, updatedJson);
                    TempData["deleteStatus"] = true;
                }
                else
                {
                    TempData["deleteStatus"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["deleteStatus"] = false;
            }
            return RedirectToAction(nameof(StudentController.Index), "Student");
        }


        [HttpGet]
        public IActionResult Edit(int id = 0)
        {
            string dataJson = System.IO.File.ReadAllText(filePathStudent);
            var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
            var student = students.Find(s => s.Id == id.ToString());

            if (student == null)
            {
                return NotFound(); // Handle case where student is not found
            }

            StudentViewModel studentViewModel = new StudentViewModel
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gender = student.Gender,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address
            };

            return View(studentViewModel);
        }

        [HttpPost, ActionName("Edit")]
        public IActionResult EditPost(StudentViewModel studentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string dataJson = System.IO.File.ReadAllText(filePathStudent);
                    var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
                    var student = students.Find(match: s => s.Id == studentViewModel.Id.ToString());

                    if (student != null)
                    {
                        // Update student properties
                        student.StudentCode = studentViewModel.StudentCode;
                        student.FirstName = studentViewModel.FirstName;
                        student.LastName = studentViewModel.LastName;
                        student.Gender = studentViewModel.Gender;
                        student.Email = studentViewModel.Email;
                        student.DateOfBirth = studentViewModel.DateOfBirth;
                        student.PhoneNumber = studentViewModel.PhoneNumber;
                        student.Address = studentViewModel.Address;

                        // Save updated data
                        string updatedJson = JsonConvert.SerializeObject(students, Formatting.Indented);
                        System.IO.File.WriteAllText(filePathStudent, updatedJson);

                        TempData["editStatus"] = true;
                        return RedirectToAction(nameof(StudentController.Index));
                    }
                    else
                    {
                        TempData["editStatus"] = false;
                    }
                }
            }
            catch
            {
                TempData["editStatus"] = false;
            }

            // If something goes wrong, return the same view with the model
            return View(studentViewModel);
        }

    }
}
