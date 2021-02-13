using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectQualityChecker.Controllers;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services.IServices;
using Xunit;

namespace Tests
{
    public class AnalysisControllerTest
    {
        [Fact]
        public void IndexTest()
        {
            var mock1 = new Mock<IRepositoryService>();
            var mock2 = new Mock<ISonarQubeScanner>();

            var controller = new AnalysisController(mock1.Object, mock2.Object);
            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Analysis_ValidFormModel()
        {
            var mock1 = new Mock<IRepositoryService>();
            mock1.Setup(r => r.Create(It.IsAny<RepositoryForm>())).Returns(new Repository());
            var mock2 = new Mock<ISonarQubeScanner>();
            mock2.Setup(s => s.ScanRepositoryAsync(It.IsAny<Repository>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var controller = new AnalysisController(mock1.Object, mock2.Object);
            var result = await controller.Analysis(new RepositoryForm());

            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Analysis_InvalidFormModel()
        {
            var mock1 = new Mock<IRepositoryService>();
            mock1.Setup(r => r.Create(It.IsAny<RepositoryForm>())).Returns(new Repository());
            var mock2 = new Mock<ISonarQubeScanner>();
            mock2.Setup(s => s.ScanRepositoryAsync(It.IsAny<Repository>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var controller = new AnalysisController(mock1.Object, mock2.Object);
            controller.ViewData.ModelState.AddModelError("Key", "ErrorMessage");
            // Setting up the situation: the controller is receiving an invalid model.

            var result = await controller.Analysis(null);

            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void Analysis_ScannerThrowException()
        {
            var mock1 = new Mock<IRepositoryService>();
            mock1.Setup(r => r.Create(It.IsAny<RepositoryForm>())).Returns(new Repository());
            var mock2 = new Mock<ISonarQubeScanner>();
            mock2.Setup(s => s.ScanRepositoryAsync(It.IsAny<Repository>(), It.IsAny<string>()))
                .Returns(Task.FromException<ApplicationException>(new ApplicationException("application_exception_from_scanner")));

            var controller = new AnalysisController(mock1.Object, mock2.Object);

            var result = await controller.Analysis(null);

            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}