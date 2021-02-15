using System.Net;
using System.Threading.Tasks;
using ProjectQualityChecker.Services;
using Xunit;

namespace Tests.UnitTests
{
    public class SonarQubeClientTests : BaseForTest
    {
        [Fact]
        public async Task CreateProject_SonarQube_API_CallTest()
        {
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var projectName = "test_project_name";

            var result = await sonarQubeClient.CreateProject(projectName);

            Assert.NotNull(result);
            Assert.Equal(projectName, result.Key);
        }

        [Fact]
        public async void DeleteProject_SonarQube_API_CallTest()
        {
            await CreateProject_SonarQube_API_CallTest();
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var projectName = "test_project_name";

            var result = await sonarQubeClient.DeleteProject(projectName);

            Assert.Equal(HttpStatusCode.NoContent, result);
        }

        [Fact]
        public async void GenerateToken_ReturnTokenToProjectTest()
        {
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var tokenName = "test_token";

            var result = await sonarQubeClient.GenerateToken(tokenName);

            Assert.NotEmpty(result.Token);
            Assert.Equal(tokenName, result.Name);
        }

        [Fact]
        public async void RevokeProjectToken_ReturnNoContentHTTPStatusTest()
        {
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var tokenName = "test_token";

            var createdToken = await sonarQubeClient.GenerateToken(tokenName);
            var result = await sonarQubeClient.RevokeToken(createdToken.Name);

            Assert.Equal(HttpStatusCode.NoContent, result);
        }
    }
}