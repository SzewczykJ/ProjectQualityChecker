using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using Moq;
using ProjectQualityChecker.Data;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.DataRepository;
using ProjectQualityChecker.Services;
using Xunit;

namespace Tests
{
    public class DeveloperRepositoryTest : BaseForTest
    {
        [Fact]
        public void TestAddingNewDeveloper()
        {
            var developerRepo = new Mock<DeveloperRepo>(GetDbContext());
            var developerService = new DeveloperService(developerRepo.Object);
            var testDeveloper = new Developer
            {
                Email = "test@developer.com",
                Username = "testDev",
                FirstName = "Jan",
                LastName = "Kowalski"
            };

            var result = developerService.Add(testDeveloper);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestEditingDeveloper()
        {
            var dbContext = GivenDeveloperRepository();

            var developerRepo = new Mock<DeveloperRepo>(dbContext);
            var developerService = new DeveloperService(developerRepo.Object);

            var developerToVerify = new Developer
            {
                Email = "test@developer.com",
                Username = "testDev_Edited",
                FirstName = "Adam",
                LastName = "Nosalski"
            };

            var selectedDeveloper = dbContext.Developers.FirstOrDefault(c => c.Email == "test@developer.com");

            selectedDeveloper.Email = developerToVerify.Email;
            selectedDeveloper.Username = developerToVerify.Username;
            selectedDeveloper.FirstName = developerToVerify.FirstName;
            selectedDeveloper.LastName = developerToVerify.LastName;

            var result = developerService.Update(selectedDeveloper);

            var developerAfterUpdate = dbContext.Developers.FirstOrDefault(c => c.Email == "test@developer.com");

            Assert.Equal(1, result);
            Assert.NotNull(developerAfterUpdate);
            Assert.Equal(developerToVerify.Email, developerAfterUpdate.Email);
            Assert.Equal(developerToVerify.Username, developerAfterUpdate.Username);
            Assert.Equal(developerToVerify.FirstName, developerAfterUpdate.FirstName);
            Assert.Equal(developerToVerify.LastName, developerAfterUpdate.LastName);
        }

        [Fact]
        public void TestRemovingDeveloper()
        {
            var dbContext = GivenDeveloperRepository();

            var developerRepo = new Mock<DeveloperRepo>(dbContext);
            var developerService = new DeveloperService(developerRepo.Object);

            var selectedDeveloper = dbContext.Developers.FirstOrDefault(c => c.Email == "testremove@developer.com");

            var result = developerService.Delete(selectedDeveloper);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestCreateDeveloperFromGitCommit()
        {
            var dbContext = GivenDeveloperRepository();

            var committerName = "test_committer_name";
            var committerEmail = "test@email.com";

            Mock<LibGit2Sharp.Commit> mockedCommit = new Mock<LibGit2Sharp.Commit>();
            mockedCommit.SetupGet(c => c.Message).Returns("test_commit_message");
            mockedCommit.SetupGet(c => c.Sha).Returns("4b825dc642cb6eb9a060e54bf8d69288fbee4904");
            var commitSignature =
                new Signature(new Identity(committerName, committerEmail), DateTimeOffset.Now);
            mockedCommit.SetupGet(c => c.Committer).Returns(commitSignature);

            var developerRepo = new Mock<DeveloperRepo>(dbContext);
            var developerService = new DeveloperService(developerRepo.Object);

            var result = developerService.CreateDeveloperFromGitCommit(mockedCommit.Object);

            Assert.NotNull(result);
            Assert.Equal(committerEmail, result.Email);
            Assert.Equal(committerName, result.Username);
        }

        private AppDbContext GivenDeveloperRepository()
        {
            var dbContext = GetDbContext();
            var devsList = new List<Developer>();
            devsList.Add(new Developer
            {
                Email = "test@developer.com",
                Username = "testDeveloperName",
                FirstName = "Jan",
                LastName = "Kowalski"
            });
            devsList.Add(new Developer
            {
                Email = "testremove@developer.com",
                Username = "testDev_Edited",
                FirstName = "Adam",
                LastName = "Nosalski"
            });
            dbContext.Developers.AddRange(devsList);
            dbContext.SaveChanges();
            return dbContext;
        }
    }
}