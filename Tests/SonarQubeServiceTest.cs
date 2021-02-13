using System.Collections.Generic;
using Moq;
using ProjectQualityChecker.Data.DataRepository;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services;
using ProjectQualityChecker.Services.IServices;
using Xunit;
using Commit = ProjectQualityChecker.Data.Database.Commit;

namespace Tests
{
    public class SonarQubeServiceTest : BaseForTest
    {
        [Fact]
        public async void TestSavingMetricForFile()
        {
            //Given
            var fileRepo = GetInMemoryFileRepo();
            var sonarQubeService = GivenSonarQubeService(fileRepo);
            var commit = new Commit();
            var dictionaries = new Dictionary<string, CommitChanges>();
            dictionaries.Add("fileName.xml", new CommitChanges());

            //When
            await sonarQubeService.SaveDataFromRepositoryAsync("projectName", commit, dictionaries);
            //Then
            var file = fileRepo.FindById(1);
            Assert.Equal(1, file.Metric.Complexity);
            Assert.Equal(2, file.Metric.CognitiveComplexity);
            Assert.Equal(3, file.Metric.DuplicatedLines);
            Assert.Equal(4, file.Metric.CodeSmells);
            Assert.Equal(8, file.Metric.NewCodeSmells);
            Assert.Equal(5, file.Metric.CommentLines);
            Assert.Equal(5.5, file.Metric.CommentLinesDensity);
            Assert.Equal(6, file.Metric.Ncloc);
            Assert.Equal(7, file.Metric.Statements);
            Assert.Equal(7.5, file.Metric.BranchCoverage);
            Assert.Equal(7.7, file.Metric.LineCoverage);
        }

        private SonarQubeService GivenSonarQubeService(IFileRepo fileRepo)
        {
            var mockSonarClient = new Mock<ISonarQubeClient>();

            mockSonarClient.Setup(m => m.GetProjectTree(It.IsAny<string>())).Returns(GivenProjectTree());
            mockSonarClient.Setup(m => m.GetMetricsForFile(It.IsAny<string>())).Returns(GivenMetrics());

            var mockFileDetail = new Mock<IFileDetailRepo>();
            var mockLanguageRepo = new Mock<ILanguageRepo>();

            return new SonarQubeService(mockSonarClient.Object, fileRepo, mockFileDetail.Object,
                mockLanguageRepo.Object);
        }

        private IFileRepo GetInMemoryFileRepo()
        {
            return new FileRepo(GetDbContext());
        }
    }
}