using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIMS_SE06205.Models;

namespace SIMS_SE06205.Controllers
{
    public class CourseController : Controller
    {
        private string filePathCourse = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-courses.json";

        [HttpGet]
        public IActionResult Index()
        {
            string dataJson = System.IO.File.ReadAllText(filePathCourse);
            CourseModel courseModel = new CourseModel();
            courseModel.CourseLists = new List<CourseViewModel>();

            // kiem tra username va password co ton tai trong dataJson hay khong ?
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            var dataCourse = (from c in courses select c).ToList();
            foreach (var item in dataCourse)
            {
                courseModel.CourseLists.Add(new CourseViewModel
                {
                    Id = item.Id,
                    NameCourse = item.NameCourse,
                    Description = item.Description
                });
            }
            return View(courseModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            CourseViewModel course = new CourseViewModel();
            return View(course);
        }

        [HttpPost]
        public IActionResult Add(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string dataJson = System.IO.File.ReadAllText(filePathCourse);
                    var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);

                    int maxId = 0;
                    if (courses != null && courses.Any())
                    {
                        // Ensure Ids are treated as integers
                        maxId = courses.Max(c => int.Parse(c.Id));
                    }
                    int newId = maxId + 1;

                    courses.Add(new CourseViewModel
                    {
                        Id = newId.ToString(),  // Storing as a string if necessary
                        NameCourse = courseViewModel.NameCourse,
                        Description = courseViewModel.Description
                    });

                    var updatedJson = JsonConvert.SerializeObject(courses, Formatting.Indented);
                    System.IO.File.WriteAllText(filePathCourse, updatedJson);

                    TempData["saveStatus"] = true;
                }
                catch (Exception ex)
                {
                    TempData["saveStatus"] = false;
                    // Log the exception details
                    System.Diagnostics.Debug.WriteLine($"Error while adding course: {ex.Message}");
                }

                return RedirectToAction(nameof(CourseController.Index), "Course");
            }

            return View(courseViewModel);
        }


        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            try
            {
                string dataJson = System.IO.File.ReadAllText(filePathCourse);
                var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
                var itemToDelete = courses.Find(item => item.Id == id.ToString());
                if (itemToDelete != null)
                {
                    courses.Remove(itemToDelete);
                    string deletedJson = JsonConvert.SerializeObject(courses, Formatting.Indented);
                    System.IO.File.WriteAllText(filePathCourse, deletedJson);
                    TempData["DeleteStatus"] = true;
                }
                else
                {
                    TempData["DeleteStatus"] = false;
                }
            }
            catch
            {
                TempData["DeleteStatus"] = false;
            }
            return RedirectToAction(nameof(CourseController.Index), "Course");
        }

        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            string dataJson = System.IO.File.ReadAllText(filePathCourse);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            var itemCourse = courses.Find(item => item.Id == id.ToString());

            CourseViewModel courseModel = new CourseViewModel();

            if (itemCourse != null)
            {
                courseModel.Id = itemCourse.Id;
                courseModel.NameCourse = itemCourse.NameCourse;
                courseModel.Description = itemCourse.Description;
            }

            if (courseModel.NameCourse == null)
            {
                return RedirectToAction(nameof(CourseController.Index), "Course");
            }
            return View(courseModel);
        }

        [HttpPost]
        public IActionResult Update(CourseViewModel courseModel)
        {
            try
            {
                string dataJson = System.IO.File.ReadAllText(filePathCourse);
                var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
                var itemCourse = courses.Find(item => item.Id == courseModel.Id.ToString());

                if (itemCourse != null)
                {
                    itemCourse.NameCourse = courseModel.NameCourse;
                    itemCourse.Description = courseModel.Description;
                    string updateJson = JsonConvert.SerializeObject(courses, Formatting.Indented);
                    System.IO.File.WriteAllText(filePathCourse, updateJson);
                    TempData["UpdateStatus"] = true;
                }
                else
                {
                    TempData["UpdateStatus"] = false;
                }
            }
            catch (Exception ex)
            {
                TempData["UpdateStatus"] = false;
            }
            return RedirectToAction(nameof(CourseController.Index), "Course");
        }
    }
}
