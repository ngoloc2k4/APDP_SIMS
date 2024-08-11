using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIMS_SE06205.Controllers;
using SIMS_SE06205.Models;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Test_SIMS
{
    public class StudentControllerTests
    {
        private readonly StudentController _controller;
        private readonly string _testFilePath = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-student-test.json"; // Test file path

        public StudentControllerTests()
        {
            // Arrange: Set up the controller and test file
            _controller = new StudentController();
            _controller.GetType().GetField("filePathStudent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                       ?.SetValue(_controller, _testFilePath);

            // Initialize the test file with empty data
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(new List<StudentViewModel>(), Formatting.Indented));
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfStudents()
        {
            // Arrange
            var testStudents = new List<StudentViewModel>
            {
                new StudentViewModel { Id = "1", StudentCode = "S001", FirstName = "John", LastName = "Doe", Gender = "Male", Address = "Hanoi", DateOfBirth = "2002-02-02", Email = "johndoe@gmail.com", PhoneNumber = "0932473248" },
                new StudentViewModel { Id = "2", StudentCode = "S002", FirstName = "Jane", LastName = "Smith", Gender = "Female", Address = "Hanoi", DateOfBirth = "2001-02-02", Email = "jane@gmail.com", PhoneNumber = "0328432794" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testStudents, Formatting.Indented));

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StudentModel>(viewResult.ViewData.Model);
            Assert.Equal(2, model.StudentsList.Count);
        }

        [Fact]
        public void Add_GET_ReturnsViewResult_WithEmptyStudentModel()
        {
            // Act
            var result = _controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StudentViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Id);
            Assert.Null(model.StudentCode);
        }

        [Fact]
        public void Add_POST_ReturnsRedirectAndSavesStudent()
        {
            // Arrange
            var newStudent = new StudentViewModel
            {
                StudentCode = "S003",
                FirstName = "Alan",
                LastName = "Makeyt",
                Gender = "Male",
                Address = "Hanoi",
                DateOfBirth = "2002-02-02",
                Email = "john@gmail.com",
                PhoneNumber = "0932473248"
            };

            // Act
            var result = _controller.Add(newStudent);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Student", redirectResult.ControllerName);

            var dataJson = File.ReadAllText(_testFilePath);
            var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
            Assert.Contains(students, s => s.StudentCode == "S003" &&
                                            s.FirstName == "Alan" &&
                                            s.LastName == "Makeyt" &&
                                            s.Gender == "Male" &&
                                            s.Address == "Hanoi" &&
                                            s.DateOfBirth == "2002-02-02" &&
                                            s.Email == "john@gmail.com" &&
                                            s.PhoneNumber == "0932473248");
        }

        [Fact]
        public void Delete_GET_RemovesStudentAndRedirects()
        {
            // Arrange
            var testStudents = new List<StudentViewModel>
            {
                new StudentViewModel { Id = "1", StudentCode = "S001", FirstName = "John", LastName = "Doe", Gender = "Male", Address = "Hanoi", DateOfBirth = "2002-02-02", Email = "johndoe@gmail.com", PhoneNumber = "0932473248" },
                new StudentViewModel { Id = "2", StudentCode = "S002", FirstName = "Jane", LastName = "Smith", Gender = "Female", Address = "Hanoi", DateOfBirth = "2001-02-02", Email = "jane@gmail.com", PhoneNumber = "0328432794" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testStudents, Formatting.Indented));

            // Act
            var result = _controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Student", redirectResult.ControllerName);

            var dataJson = File.ReadAllText(_testFilePath);
            var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
            Assert.DoesNotContain(students, s => s.Id == "1");
        }

        [Fact]
        public void Edit_GET_ReturnsViewResult_WithStudentModel()
        {
            // Arrange
            var testStudents = new List<StudentViewModel>
            {
                new StudentViewModel { Id = "1", StudentCode = "S001", FirstName = "John", LastName = "Doe", Gender = "Male", Address = "Hanoi", DateOfBirth = "2002-02-02", Email = "johndoe@gmail.com", PhoneNumber = "0932473248" },
                new StudentViewModel { Id = "2", StudentCode = "S002", FirstName = "Jane", LastName = "Smith", Gender = "Female", Address = "Hanoi", DateOfBirth = "2001-02-02", Email = "jane@gmail.com", PhoneNumber = "0328432794" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testStudents, Formatting.Indented));

            // Act
            var result = _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<StudentViewModel>(viewResult.ViewData.Model);
            Assert.Equal("1", model.Id);
            Assert.Equal("S001", model.StudentCode);
            Assert.Equal("John", model.FirstName);
            Assert.Equal("Doe", model.LastName);
            Assert.Equal("Male", model.Gender);
            Assert.Equal("Hanoi", model.Address);
            Assert.Equal("2002-02-02", model.DateOfBirth);
            Assert.Equal("johndoe@gmail.com", model.Email);
            Assert.Equal("0932473248", model.PhoneNumber);
        }

        [Fact]
        public void Edit_POST_ReturnsRedirectAndUpdatesStudent()
        {
            // Arrange
            var testStudents = new List<StudentViewModel>
            {
                new StudentViewModel { Id = "1", StudentCode = "S001", FirstName = "John", LastName = "Doe" }
            };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testStudents, Formatting.Indented));

            var updatedStudent = new StudentViewModel
            {
                Id = "1",
                StudentCode = "S001",
                FirstName = "Mike",
                LastName = "Doe",
            };

            // Act
            var result = _controller.EditPost(updatedStudent);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Student", redirectResult.ControllerName);

            var dataJson = File.ReadAllText(_testFilePath);
            var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
            var student = students.Find(s => s.Id == "1");
            Assert.Equal("Mike", student.FirstName);
            Assert.Equal("Doe", student.LastName);
        }
    }
}
