using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectQualityChecker.Controllers;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services.IServices;
using Xunit;

namespace Tests.UnitTests
{
    public class ResultControllerTest
    {
        [Fact]
        public void IndexTest()
        {
            var mock1 = new Mock<IResultService>();
            var mock2 = new Mock<IRepositoryService>();

            mock2.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<List<Repository>>(new List<Repository>()));
            var controller = new ResultController(mock1.Object, mock2.Object);

            var result = controller.Index();

            var okResult = Assert.IsType<ViewResult>(result.Result);
            var product = Assert.IsType<ListRepositories>(okResult.Model);
        }

        [Fact]
        public void GetResultTest()
        {
            var mock1 = new Mock<IResultService>();
            var mock2 = new Mock<IRepositoryService>();
            var repositoryId = 1;
            mock1.Setup(r => r.Summary(repositoryId)).Returns(new CommitSummaryList {CommitList = new List<CommitSummary>()});

            var controller = new ResultController(mock1.Object, mock2.Object);

            var result = controller.GetResult(repositoryId);

            Assert.IsType<JsonResult>(result);
            var json = result as JsonResult;
            Assert.NotNull(json);
            Assert.IsType<CommitSummaryList>(json.Value);
        }
    }
}