using System;
using System.IO;
using LibGit2Sharp;
using Moq;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Services;
using Xunit;

namespace Tests
{
    public class RepositoryServiceTest
    {
        [Fact]
        public void TestRepositoryService_TypeIsMaven()
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
        public void TestRepositoryService_TypeIsGradle()
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
        public void TestRepositoryService_TestTypeIsMS()
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
        public void TestRepositoryService_TestTypeIsOther()
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
        public void TestGetUserNameFromRepositoryUrl()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/githubname/project";

            //When
            var result = repositoryService.GetUserNameFromRepositoryUrl(URLToProject);
            //Then

            Assert.Equal("githubname", result);
        }

        [Fact]
        public void TestGetRepositoryNameFromRepositoryUrl()
        {
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/githubname/project";

            //When
            var result = repositoryService.GetRepositoryNameFromRepositoryUrl(URLToProject);
            //Then

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
            //Given
            var repositoryRepo = new Mock<IRepositoryRepo>().Object;
            var repositoryService = new RepositoryService(repositoryRepo);
            var URLToProject = "https://github.com/github/training-kit";

            var result = repositoryService.CloneRepository(URLToProject);

            Assert.IsType(typeof(Repository), result);
        }

        private string GetPathToTestDirectory(string dirName)
        {
            var fullName = new DirectoryInfo(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            return fullName + @"\Resources\" + dirName;
        }
    }
}