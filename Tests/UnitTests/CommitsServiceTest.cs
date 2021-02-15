using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using Moq;
using ProjectQualityChecker.Data;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.DataRepository;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models.Result;
using ProjectQualityChecker.Services;
using Xunit;
using Commit = ProjectQualityChecker.Data.Database.Commit;
using Repository = ProjectQualityChecker.Data.Database.Repository;

namespace Tests.UnitTests
{
    public class CommitServiceTest : BaseForTest
    {
        [Fact]
        public void TestAddingNewCommit()
        {
            var commitRepo = new Mock<CommitRepo>(GetDbContext());
            var commitService = new CommitService(commitRepo.Object);
            var exampleCommit = new Commit
            {
                Sha = "4b825dc642cb6eb9a060e54bf8d69288fbee4904",
                Date = DateTime.Now,
                Message = "Example Commit"
            };

            var result = commitService.Add(exampleCommit);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestEditingCommit()
        {
            var dbContext = GivenCommitRepository();
            var commitRepo = new Mock<CommitRepo>(dbContext);

            var commitService = new CommitService(commitRepo.Object);

            var commitToVerify = new Commit
            {
                Date = DateTime.UnixEpoch,
                Message = "Message_to_verify",
                Sha = "4b825dc642cb6eb9a060e54bf8d69288fbee7898"
            };

            var storedCommit = dbContext.Commits.FirstOrDefault(c => c.Message == "Example_Commit_To_Remove");

            storedCommit.Message = commitToVerify.Message;
            storedCommit.Date = commitToVerify.Date;
            storedCommit.Sha = commitToVerify.Sha;

            var result = commitService.Update(storedCommit);

            var commitAfterEdit = dbContext.Commits.FirstOrDefault(c => c.CommitId.Equals(storedCommit.CommitId));

            Assert.Equal(1, result);
            Assert.Equal(commitToVerify.Message, commitAfterEdit.Message);
            Assert.Equal(commitToVerify.Date, commitAfterEdit.Date);
            Assert.Equal(commitToVerify.Sha, commitAfterEdit.Sha);
        }

        [Fact]
        public void TestRemovingCommit()
        {
            var dbContext = GivenCommitRepository();
            var commitRepo = new Mock<CommitRepo>(dbContext);

            var commitService = new CommitService(commitRepo.Object);
            var storedCommit = dbContext.Commits.FirstOrDefault(c => c.Message.Equals("Example_Commit_To_Remove"));

            var result = commitService.Delete(storedCommit);

            Assert.Equal(1, result);
        }

        [Fact]
        public void TestGetCommitSummaries()
        {
            var dbContext = GivenCommitRepsoitoryContextIncludedRelatedData();
            var commitRepo = new Mock<CommitRepo>(dbContext);

            var commitService = new CommitService(commitRepo.Object);
            var storedRepository = dbContext.Repositories.FirstOrDefault(r => r.FullName.Equals("ExampleRepository"));

            var result = commitService.GetCommitSummaries((int) storedRepository.RepositoryId);

            Assert.NotNull(result);
            Assert.IsType<CommitSummaryList>(result);
            Assert.NotEmpty(result.CommitList);
        }

        [Fact]
        public void TestGenerateCommitFromGitCommitInfo()
        {
            Mock<ICommitRepo> mockCommitRepo = new Mock<ICommitRepo>();
            Mock<LibGit2Sharp.Commit> mockedCommit = new Mock<LibGit2Sharp.Commit>();
            mockedCommit.SetupGet(c => c.Message).Returns("test_commit_message");
            mockedCommit.SetupGet(c => c.Sha).Returns("4b825dc642cb6eb9a060e54bf8d69288fbee4904");

            var commitSignature =
                new Signature(new Identity("test_committer_name", "test@email.com"), DateTimeOffset.Now);
            mockedCommit.SetupGet(c => c.Author).Returns(commitSignature);

            var commitService = new CommitService(mockCommitRepo.Object);
            Mock<Repository> mockRepository = new Mock<Repository>();
            Developer developer = new Developer {Email = "test@email.com", Username = "DeveloperName"};

            var result = commitService.GenerateCommitFromGitCommitInfo(mockedCommit.Object, mockRepository.Object, developer);

            Assert.NotNull(result);
            Assert.Equal("test_commit_message", result.Message);
            Assert.Equal("4b825dc642cb6eb9a060e54bf8d69288fbee4904", result.Sha);
        }

        private AppDbContext GivenCommitRepository()
        {
            var dbContext = GetDbContext();
            var exampleCommit = new Commit
            {
                Sha = "4b825dc642cb6eb9a060e54bf8d69288fbee4904",
                Date = DateTime.Now,
                Message = "Example_Commit_To_Remove"
            };
            dbContext.Commits.Add(exampleCommit);
            dbContext.SaveChanges();
            return dbContext;
        }

        private AppDbContext GivenCommitRepsoitoryContextIncludedRelatedData()
        {
            var dbContext = GetDbContext();

            var exampleCommitList = new List<Commit>();

            var givenFiles = new List<File>();
            givenFiles.Add(new File
            {
                FileDetail = new FileDetail
                {
                    Extension = "java",
                    Name = "example.java"
                },
                Metric = new Metric
                {
                    Date = DateTime.Now,
                    Complexity = 1,
                    Statements = 5,
                    BranchCoverage = 0.5,
                    CommentLines = 23,
                    CommentLinesDensity = 0.9
                }
            });

            exampleCommitList.Add(new Commit
            {
                Sha = "4b825dc642cb6eb9a060e54bf8d69288fbee4904",
                Date = DateTime.Now,
                Message = "Example_Commit_To_Remove",
                Developer = new Developer
                {
                    Email = "example@dev.com",
                    Username = "exampleDevName"
                },
                Files = givenFiles
            });

            var givenRepository = new Repository
            {
                FullName = "ExampleRepository",
                Name = "ExampleRepositoryName",
                Commits = exampleCommitList
            };
            dbContext.Repositories.Add(givenRepository);
            dbContext.SaveChanges();
            return dbContext;
        }
    }
}