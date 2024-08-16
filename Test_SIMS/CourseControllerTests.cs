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
        private readonly string _testFilePath = @"F:\SIMS_practice\APDP-BTec-main\data-sims\data-courses-test.json"; // Đường dẫn tới tệp kiểm thử

        public CourseControllerTests()
        {
            _controller = new CourseController();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Đảm bảo tệp kiểm thử tồn tại và khởi tạo nó
            if (!File.Exists(_testFilePath))
            {
                // Khởi tạo dữ liệu ban đầu
                var initialData = new List<CourseViewModel>();
                // Ghi dữ liệu vào tệp
                File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        [Fact]
        public void Index_ReturnsViewResult_WithCourses()
        {
            var dataInFile = new List<CourseViewModel>
            { };
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(dataInFile, Formatting.Indented));

            // Thực thi
            var result = _controller.Index();

            // Kiểm tra
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseModel>(viewResult.ViewData.Model);

            // Kết quả: liệt kê tất cả các khóa học
            Assert.Empty(model.CourseLists);
        }

        [Fact]
        public void Add_GET_ReturnsViewResult_WithEmptyCourseModel()
        {
            // Thực thi
            var result = _controller.Add();

            // Kiểm tra
            var viewResult = Assert.IsType<ViewResult>(result);

            // Kết quả: trả về một mô hình CourseViewModel trống
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);

            // Xác nhận rằng mô hình trả về là trống
            Assert.Null(model.Id);
            Assert.Null(model.NameCourse);
            Assert.Null(model.Description);
        }

        [Fact]
        public void Add_POST_ReturnsRedirectAndSavesCourse()
        {
            // Chuẩn bị
            var newCourse = new CourseViewModel
            {
                NameCourse = "New Course 4",
                Description = "New Course Description 4444"
            };
            // Ghi dữ liệu vào tệp
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(new List<CourseViewModel>(), Formatting.Indented));

            // Thực thi
            var result = _controller.Add(newCourse);

            // Kiểm tra
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);
            // Kiểm tra xem khóa học đã được thêm vào tệp chưa
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            Assert.Contains(courses, c => c.NameCourse == "New Course 4" && c.Description == "New Course Description 4444");
        }

        [Fact]
        public void Update_GET_ReturnsViewResult_WithCourseModel()
        {
            // Chuẩn bị
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Old Course", Description = "Old Description" }
            };
            // Ghi dữ liệu vào tệp
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));

            // Thực thi
            var result = _controller.Update(1);

            // Kiểm tra
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            // Xác nhận rằng mô hình trả về chứa thông tin của khóa học cần cập nhật
            Assert.Equal("1", model.Id);
            Assert.Equal("Old Course", model.NameCourse);
            Assert.Equal("Old Description", model.Description);
        }

        [Fact]
        public void Update_POST_ReturnsRedirectAndUpdatesCourse()
        {
            // Chuẩn bị
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Old Course", Description = "Old Description" }
            };
            // Ghi dữ liệu vào tệp
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));
            // Cập nhật thông tin khóa học
            var updatedCourse = new CourseViewModel
            {
                Id = "1",
                NameCourse = "Updated Course",
                Description = "Updated Description"
            };

            // Thực thi
            var result = _controller.Update(updatedCourse);

            // Kiểm tra
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            // Kiểm tra xem có chuyển hướng đến trang Index không
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);
            // Kiểm tra xem khóa học đã được cập nhật trong tệp chưa
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            var course = courses.Find(c => c.Id == "1");
            Assert.Equal("Updated Course", course.NameCourse);
            Assert.Equal("Updated Description", course.Description);
        }

        [Fact]
        public void Delete_DeletesCourseAndRedirects()
        {
            // Chuẩn bị
            var testCourses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = "1", NameCourse = "Old Course 1", Description = "Old Description 1" },
                new CourseViewModel { Id = "2", NameCourse = "Old Course 2", Description = "Old Description 2" }
            };
            // Ghi dữ liệu vào tệp
            File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testCourses, Formatting.Indented));
            // Mock TempData
            var mockTempData = new Mock<ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            // Thực thi
            var result = _controller.Delete(1);

            // Kiểm tra
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Course", redirectResult.ControllerName);
            // Kiểm tra xem khóa học đã được xóa khỏi tệp chưa
            var dataJson = File.ReadAllText(_testFilePath);
            var courses = JsonConvert.DeserializeObject<List<CourseViewModel>>(dataJson);
            Assert.DoesNotContain(courses, c => c.Id == "1");
            Assert.Contains(courses, c => c.Id == "2");
        }

        [Fact]
        public void Add_POST_InvalidModel_ReturnsViewResult()
        {
            // Chuẩn bị
            _controller.ModelState.AddModelError("NameCourse", "Required");

            var invalidCourse = new CourseViewModel
            {
                // Không đặt NameCourse, gây ra lỗi xác nhận
            };

            // Thực thi
            var result = _controller.Add(invalidCourse);

            // Kiểm tra
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            // Xác nhận rằng mô hình trả về chứa thông tin của khóa học không hợp lệ
            Assert.Equal(invalidCourse, model);
        }

        [Fact]
        public void Add_POST_EmptyNameCourse_ReturnsViewResult()
        {
            // Chuẩn bị
            _controller.ModelState.AddModelError("NameCourse", "Required");

            var invalidCourse = new CourseViewModel
            {
                NameCourse = "", // Đặt NameCourse rỗng để gây ra lỗi xác nhận
                Description = "Some Description"
            };

            // Thực thi
            var result = _controller.Add(invalidCourse);

            // Kiểm tra
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CourseViewModel>(viewResult.ViewData.Model);
            // Xác nhận rằng mô hình trả về chứa thông tin của khóa học không hợp lệ
            Assert.Equal("", model.NameCourse); // Xác nhận đầu vào không hợp lệ
            Assert.Equal("Some Description", model.Description);
        }
    }
}
