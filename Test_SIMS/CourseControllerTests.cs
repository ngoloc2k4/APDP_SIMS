using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using SIMS_SE06205.Controllers;
using SIMS_SE06205.Models;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Test_SIMS
{
    public class CourseControllerTests
    {

        private readonly CourseController _controller;
        private readonly string _testFilePath = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-courses-test.json"; // Test file

        public CourseControllerTests()
        {
            _controller = new CourseController();


            // Ensure the test file exists and initialize it
            if (!File.Exists(_testFilePath))
            {
                var initialData = new List<CourseViewModel>();
                File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfCourses()
        {
            // Arrange: Prepare the test data
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Course 1", Description = "Description 1" },
                new CourseViewModel { Id = "2", NameCourse = "Course 2", Description = "Description 2" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            // Act: Call the method under test
            var result = _controller.Index();

            // Assert: Verify the results
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.CourseLists.Count);
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
                NameCourse = "New Course",
                Description = "New Course Description"
            };

            // Act
            var result = _controller.Add(newCourse);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);

            // Verify that the course was added
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            Assert.Contains(courses, c => c.NameCourse == "New Course" && c.Description == "New Course Description");
        }

        [Fact]
        public void Delete_GET_RemovesCourseAndRedirects()
        {
            // Arrange
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Course 1", Description = "Description 1" },
                new CourseViewModel { Id = "2", NameCourse = "Course 2", Description = "Description 2" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            // Act
            var result = _controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);

            // Verify that the course was deleted
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            Assert.DoesNotContain(courses, c => c.Id == "1");
        }

        [Fact]
        public void Update_GET_ReturnsViewResult_WithCourseModel()
        {
            // Arrange
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Course 1", Description = "Description 1" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            // Act
            var result = _controller.Update(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            Assert.Equal("1", model.Id);
            Assert.Equal("Course 1", model.NameCourse);
            Assert.Equal("Description 1", model.Description);
        }

        [Fact]
        public void Update_POST_ReturnsRedirectAndUpdatesCourse()
        {
            // Arrange
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Course 1", Description = "Description 1" }
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

            // Verify that the course was updated
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            var course = courses.Find(c => c.Id == "1");
            Assert.Equal("Updated Course", course.NameCourse);
            Assert.Equal("Updated Description", course.Description);

        }


    }
}