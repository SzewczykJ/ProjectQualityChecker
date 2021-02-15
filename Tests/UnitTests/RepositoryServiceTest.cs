using System;
using System.IO;
using LibGit2Sharp;
using Moq;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Services;
using Xunit;

namespace Tests.UnitTests
{
    public class RepositoryServiceTest
    {
        [Fact]
        public void RepositoryTypeIsMavenTest()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var pathToProject = GetPathToTestDirectory("Maven");

            //When
            var repositoryType = repositoryService.GetRepositoryType(pathToProject);
            //Then

            Assert.Equal(RepositoryService.RepositoryType.MAVEN, repositoryType);
        }

        [Fact]
        public void RepositoryTypeIsGradleTest()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var pathToProject = GetPathToTestDirectory("Gradle");

            //When
            var repositoryType = repositoryService.GetRepositoryType(pathToProject);
            //Then

            Assert.Equal(RepositoryService.RepositoryType.GRADLE, repositoryType);
        }

        [Fact]
        public void RepositoryTypeIsMSTest()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var pathToProject = GetPathToTestDirectory("Csharp");

            //When
            var repositoryType = repositoryService.GetRepositoryType(pathToProject);
            //Then

            Assert.Equal(RepositoryService.RepositoryType.MS, repositoryType);
        }

        [Fact]
        public void RepositoryTypeIsOtherTest()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var pathToProject = GetPathToTestDirectory("Other");

            //When
            var repositoryType = repositoryService.GetRepositoryType(pathToProject);
            //Then

            Assert.Equal(RepositoryService.RepositoryType.OTHER, repositoryType);
        }

        [Fact]
        public void GetUserNameFromRepositoryUrlTest()
        {
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/githubname/project";

            var result = repositoryService.GetUserNameFromRepositoryUrl(URLToProject);

            Assert.Equal("githubname", result);
        }

        [Fact]
        public void GetRepositoryNameFromRepositoryUrlTest()
        {
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/githubname/project";

            var result = repositoryService.GetRepositoryNameFromRepositoryUrl(URLToProject);

            Assert.Equal("project", result);
        }

        [Fact]
        public void CloneRepository_TestPrivateRepo_ThrowApplicationException()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/SzewczykJ/ProjectQualityChecker";

            Assert.Throws<ApplicationException>(() => repositoryService.CloneRepository(URLToProject));
        }

        [Fact]
        public void TestCloneRepository_CloneRepoToDirectory()
        {
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/github/training-kit";

            var result = repositoryService.CloneRepository(URLToProject);

            Assert.IsType(typeof(Repository), result);
        }

        private string GetPathToTestDirectory(string dirName)
        {
            var fullName = new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            return fullName + @"/Resources/" + dirName;
        }
    }
}