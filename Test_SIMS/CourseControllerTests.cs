using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using SIMS_SE06205.Controllers;
using SIMS_SE06205.Models;

namespace Test_SIMS
{
    public class CourseControllerTests
    {

        private readonly CourseController _controller;
        private readonly string _testFilePath = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-courses-test.json"; // Test file

        public CourseControllerTests()
        {
            _controller = new CourseController();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());


            // Ensure the test file exists and initialize it
            if (!File.Exists(_testFilePath))
            {
                var initialData = new List<CourseViewModel>();
                File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        [Fact]
        public void Index_ReturnsViewResult_WithCourses()
        {
            var dataInFile = new List<CourseViewModel>
            { };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(dataInFile, Formatting.Indented));

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseModel>(viewResult.ViewData.Model);

            //Result: list all courses
            Assert.Empty(model.CourseLists);



        }

        [Fact]
        public void Add_GET_ReturnsViewResult_WithEmptyCourseModel()
        {
            // Act
            var result = _controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Id);
            Assert.Null(model.NameCourse);
            Assert.Null(model.Description);
        }


        [Fact]
        public void Add_POST_ReturnsRedirectAndSavesCourse()
        {
            // Arrange
            var newCourse = new CourseViewModel
            {
                NameCourse = "New Course 4",
                Description = "New Course Description 4444"
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(new List<CourseViewModel>(), Formatting.Indented));

            // Act
            var result = _controller.Add(newCourse);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);

            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            Assert.Contains(courses, c => c.NameCourse == "New Course 4" && c.Description == "New Course Description 4444");
        }


        [Fact]
        public void Update_GET_ReturnsViewResult_WithCourseModel()
        {
            // Arrange
            var testCourses = new List<CourseViewModel>
    {
        new CourseViewModel { Id = "1", NameCourse = "Old Course", Description = "Old Description" }
    };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            // Act
            var result = _controller.Update(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Equal("1", model.Id);
            Assert.Equal("Old Course", model.NameCourse);
            Assert.Equal("Old Description", model.Description);
        }

        [Fact]
        public void Update_POST_ReturnsRedirectAndUpdatesCourse()
        {
            // Arrange
            var testCourses = new List<CourseViewModel>
    {
        new CourseViewModel { Id = "1", NameCourse = "Old Course", Description = "Old Description" }
    };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            var updatedCourse = new CourseViewModel
            {
                Id = "1",
                NameCourse = "Updated Course",
                Description = "Updated Description"
            };

            // Act
            var result = _controller.Update(updatedCourse);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);

            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            var course = courses.Find(c => c.Id == "1");
            Assert.Equal("Updated Course", course.NameCourse);
            Assert.Equal("Updated Description", course.Description);
        }


        [Fact]
        public void Delete_DeletesCourseAndRedirects()
        {
            // Arrange
            var testCourses = new List<CourseViewModel>
    {
        new CourseViewModel { Id = "1", NameCourse = "Old Course 1", Description = "Old Description 1" },
        new CourseViewModel { Id = "2", NameCourse = "Old Course 2", Description = "Old Description 2" }
    };

            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            var mockTempData = new Mock<ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            // Act
            var result = _controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);

            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            Assert.DoesNotContain(courses, c => c.Id == "1");
            Assert.Contains(courses, c => c.Id == "2");
        }





        [Fact]
        public void Add_POST_InvalidModel_ReturnsViewResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("NameCourse", "Required");

            var invalidCourse = new CourseViewModel
            {
                // No NameCourse set, triggering validation error
            };

            // Act
            var result = _controller.Add(invalidCourse);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Equal(invalidCourse, model);
        }
        [Fact]
        public void Add_POST_EmptyNameCourse_ReturnsViewResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("NameCourse", "Required");

            var invalidCourse = new CourseViewModel
            {
                NameCourse = "", // Empty NameCourse to trigger validation error
                Description = "Some Description"
            };

            // Act
            var result = _controller.Add(invalidCourse);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Equal("", model.NameCourse); // Validate the invalid input
            Assert.Equal("Some Description", model.Description);
        }

        [Fact]
        public void Update_POST_EmptyNameCourse_ReturnsViewResult()
        {
            // Arrange
            var existingCourse = new CourseViewModel
            {
                Id = "1",
                NameCourse = "Existing Course",
                Description = "Existing Description"
            };

            // Initialize the test file with the existing course
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(new List<CourseViewModel> { existingCourse }, Formatting.Indented));

            // Create an instance of the controller
            var mockTempData = new Mock<ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            var updatedCourse = new CourseViewModel
            {
                Id = "1",
                NameCourse = "", // Empty NameCourse
                Description = "Updated Description"
            };

            // Act
            var result = _controller.Update(updatedCourse);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(updatedCourse, viewResult.Model); // Check if the model is returned with the invalid data

            // Verify that the course data was not updated
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            var course = courses.Find(c => c.Id == "1");
            Assert.NotNull(course); // Ensure the course still exists
            Assert.Equal("Existing Course", course.NameCourse); // Ensure the name has not been changed
            Assert.Equal("Existing Description", course.Description); // Ensure the description remains unchanged
        }


    }
}