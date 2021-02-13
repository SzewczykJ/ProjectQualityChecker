using System.Net;
using System.Threading.Tasks;
using ProjectQualityChecker.Services;
using Xunit;

namespace Tests
{
    public class SonarQubeClientTests : BaseForTest
    {
        [Fact]
        public async Task TestCreateProject_SonarQube_API_Call()
        {
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var projectName = "test_project_name";

            var result = await sonarQubeClient.CreateProject(projectName);

            Assert.NotNull(result);
            Assert.Equal(projectName, result.Key);
        }

        [Fact]
        public async void TestDeleteProject_SonarQube_API_Call()
        {
            await TestCreateProject_SonarQube_API_Call();
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var projectName = "test_project_name";

            var result = await sonarQubeClient.DeleteProject(projectName);

            Assert.Equal(HttpStatusCode.NoContent, result);
        }

        [Fact]
        public async void TestGenerateToken_ReturnTokenToProject()
        {
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var tokenName = "test_token";

            var result = await sonarQubeClient.GenerateToken(tokenName);

            Assert.NotEmpty(result.Token);
            Assert.Equal(tokenName, result.Name);
        }

        [Fact]
        public async void TestRevokeProjectToken_ReturnNoContentHTTPStatus()
        {
            var sonarQubeClient = new SonarQubeClient(GivenHttpClient());
            var tokenName = "test_token";

            var createdToken = await sonarQubeClient.GenerateToken(tokenName);
            var result = await sonarQubeClient.RevokeToken(createdToken.Name);

            Assert.Equal(HttpStatusCode.NoContent, result);
        }
    }
}