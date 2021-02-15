using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Moq;
using ProjectQualityChecker.Data.DataRepository;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Services;
using ProjectQualityChecker.Services.IServices;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace Tests.UnitTests
{
    public class SonarQubeScannerTest : BaseForTest
    {
        /*
        [Fact]
        public async void TestCreatingRepository()
        {
            //Given
            var repositoryRepo = GetInMemoryRepositoryRepo();
            ISonarQubeScanner scanerService = GivenScanerService(repositoryRepo);
            var repositoryUrl = "https://github.com/username/repository";
            var inputRepo = new Repository
            {
                Name = "test_repository",
                Url = repositoryUrl
            };

            //When
            await scanerService.ScanRepositoryAsync(inputRepo);


            //Then
            var repository = repositoryRepo.GetById(1);
            Assert.NotNull(repository);
            Assert.False(repository.Private);
        }
*/
        private SonarQubeScanner GivenScanerService(IRepositoryRepo repositoryRepo)
        {
            var mockSonarQubeClient = new Mock<ISonarQubeClient>();
            mockSonarQubeClient.Setup(m => m.CreateProject(It.IsAny<string>())).Returns(GivenCreateProject());
            mockSonarQubeClient.Setup(m => m.GenerateToken(It.IsAny<string>())).Returns(GivenCreateToken());

            var mockRepository = new Mock<IRepositoryService>();
            mockRepository.Setup(m => m.CloneRepository(It.IsAny<string>())).Returns(GivenRepository());
            mockRepository.Setup(m => m.Update(It.IsAny<Repository>()))
                .Callback<Repository>(r => repositoryRepo.Update(r));

            var mockSonarQubeService = new Mock<ISonarQubeService>();
            var mockRepositoryService = new Mock<IRepositoryService>();
            var mockCommitService = new Mock<ICommitService>();
            var mockDeveloperService = new Mock<IDeveloperService>();
            var mockBranchRepo = new Mock<IBranchRepo>();

            var mockLibGit2SharpIRepository = new Mock<LibGit2Sharp.IRepository>();

            var mockSonarQubeScanner = new Mock<SonarQubeScanner>(
                mockSonarQubeClient.Object,
                mockRepository.Object,
                mockSonarQubeService.Object,
                mockCommitService.Object,
                mockDeveloperService.Object,
                mockBranchRepo.Object);
            mockSonarQubeScanner.CallBase = true;
            mockSonarQubeScanner.Setup(s => s.ScanAllCommitsFromRepositoryAsync(mockLibGit2SharpIRepository.Object.Commits.ToArray(),
                It.IsAny<Repository>(), It.IsAny<string>(), mockLibGit2SharpIRepository.Object, It.IsAny<string>())).Returns(Task.CompletedTask);
            return mockSonarQubeScanner.Object;
        }

        private IRepository GivenRepository()
        {
            var mockRepository = new Mock<IRepository>();
            var mockCommit = new Mock<Commit>();
            mockCommit.SetupGet(c => c.Message).Returns("test_commit_message");

            var commitSignature =
                new Signature(new Identity("test_committer_name", "test@email.com"), DateTimeOffset.Now);

            mockCommit.SetupGet(c => c.Committer).Returns(commitSignature);


            var commits = new List<Commit>();
            commits.Add(mockCommit.Object);

            var com = new Mock<IQueryableCommitLog>();
            com.Setup(c => c.GetEnumerator()).Returns(commits.GetEnumerator());

            mockRepository.SetupGet(r => r.Commits).Returns(com.Object);
            mockRepository.SetupGet(r => r.Head.FriendlyName).Returns("test_branch");

            return mockRepository.Object;
        }

        private IRepositoryRepo GetInMemoryRepositoryRepo()
        {
            return new RepositoryRepo(GetDbContext());
        }
    }
}