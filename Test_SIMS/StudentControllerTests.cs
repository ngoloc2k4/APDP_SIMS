using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using SIMS_SE06205.Controllers;
using SIMS_SE06205.Models;

namespace Test_SIMS
{
    public class StudentControllerTests
    {
        private readonly StudentController _controller;
        private readonly string _testFilePath = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-student-test.json"; // Test file path

        public StudentControllerTests()
        {
            // Mock TempData or other dependencies if needed
            var tempData = new Mock<ITempDataDictionary>();
            _controller = new StudentController
            {
                TempData = tempData.Object
            };
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

       


        /*
                 * Explanation
        Arrange:

        You create an incompleteStudent object where you deliberately omit required fields (like StudentCode, FirstName, etc.).
        Act:

        You call the Add method on your controller, passing the incomplete student.
        Assert:

        ViewResult Check: Verify that the action returns a ViewResult (i.e., it should not redirect to the index page because the operation should fail).
        ModelState Check: Ensure that ModelState is invalid, which means the validation detected the missing required fields.
        No Student Added Check: Verify that the incomplete student was not added to the JSON file.
        */


        [Fact]
        public void Add_POST_ReturnsRedirectAndSavesStudent()
        {
            // Arrange
            var newStudent = new StudentViewModel
            {
                StudentCode = "S003",
                FirstName = "Alice",
                LastName = "Johnson",
                Gender = "Female",
                Address = "123 Main St",
                DateOfBirth = "2003-03-03",
                Email = "alice.johnson@example.com",
                PhoneNumber = "0987654321"
            };

            // Act
            var result = _controller.Add(newStudent);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Student", redirectResult.ControllerName);

            // Verify that the student was added
            var dataJson = File.ReadAllText(_testFilePath);
            var students = JsonConvert.DeserializeObject<List<StudentViewModel>>(dataJson);
            Assert.Contains(students, s => s.StudentCode == "S003" &&
                                            s.FirstName == "Alice" &&
                                            s.LastName == "Johnson" &&
                                            s.Gender == "Female" &&
                                            s.Address == "123 Main St" &&
                                            s.DateOfBirth == "2003-03-03" &&
                                            s.Email == "alice.johnson@example.com" &&
                                            s.PhoneNumber == "0987654321");
        }

        /*
         Explanation
        Arrange:

        You create a newStudent object with all required fields populated. This ensures that the ModelState will be valid when the controller processes this input.
        Act:

        You call the Add method on your controller, passing the newStudent object.
        Assert:

        RedirectResult Check: Ensure that the action returns a RedirectToActionResult, which indicates the student was successfully added and the user is redirected to the "Index" page.
        Student Added Check: Read the JSON data file and ensure that the new student with all the provided details is present.
        */



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

       

    }
}
