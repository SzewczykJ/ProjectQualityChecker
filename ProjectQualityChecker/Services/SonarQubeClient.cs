using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectQualityChecker.Models;
using ProjectQualityChecker.Services.IServices;

namespace ProjectQualityChecker.Services
{
    public class SonarQubeClient : ISonarQubeClient
    {
        private static readonly string[] metricKeys =
        {
            "complexity",
            "cognitive_complexity",
            "duplicated_lines",
            "code_smells",
            "new_code_smells",
            "comment_lines",
            "comment_lines_density",
            "ncloc",
            "statements",
            "branch_coverage",
            "line_coverage"
        };

        private readonly HttpClient _httpClient;

        private readonly string metricKeysParam = string.Join(",", metricKeys);
        private readonly byte[] _credentials;

        public SonarQubeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _credentials = Encoding.ASCII.GetBytes("admin:adminq");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(_credentials));
        }

        public async Task<Root> GetMetricsForFile(string fileKey)
        {
            return JsonConvert.DeserializeObject<Root>(
                await _httpClient.GetStringAsync(
                    $"measures/component?component={fileKey}&metricKeys={metricKeysParam}"));
        }

        public async Task<ProjectTree> GetProjectTree(string projectName)
        {
            var response = await _httpClient.GetStringAsync($"components/tree?component={projectName}");
            return JsonConvert.DeserializeObject<ProjectTree>(response);
        }

        public async Task<Project> CreateProject(string projectName)
        {
            var parameters = new Dictionary<string, string>
            {
                {"name", projectName},
                {"project", projectName}
            };

            var httpResponseMessage =
                await _httpClient.PostAsync("projects/create", new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                return ExistingProject(projectName);

            var tmp = await httpResponseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SonarQubeProject>(tmp).Project;
        }

        public async Task<HttpStatusCode> DeleteProject(string projectName)
        {
            var parameters = new Dictionary<string, string>
            {
                {"project", projectName}
            };

            var httpResponseMessage =
                await _httpClient.PostAsync("projects/delete", new FormUrlEncodedContent(parameters));
            return httpResponseMessage.StatusCode;
        }

        public async Task<ProjectToken> GenerateToken(string tokenName)
        {
            var parameters = new Dictionary<string, string>
            {
                {"name", tokenName}
            };


            var httpResponseMessage =
                await _httpClient.PostAsync("user_tokens/generate", new FormUrlEncodedContent(parameters));

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                await RevokeToken(tokenName);
                httpResponseMessage =
                    await _httpClient.PostAsync("user_tokens/generate", new FormUrlEncodedContent(parameters));
                return JsonConvert.DeserializeObject<ProjectToken>(await httpResponseMessage.Content
                    .ReadAsStringAsync());
            }

            return JsonConvert.DeserializeObject<ProjectToken>(await httpResponseMessage.Content.ReadAsStringAsync());
        }

        public async Task<HttpStatusCode> RevokeToken(string tokenName)
        {
            var parameters = new Dictionary<string, string>
            {
                {"name", tokenName}
            };

            var httpResponseMessage =
                await _httpClient.PostAsync("user_tokens/revoke", new FormUrlEncodedContent(parameters));
            return httpResponseMessage.StatusCode;
        }

        private Project ExistingProject(string projectKey)
        {
            return new Project {Key = projectKey, Name = projectKey};
        }
    }
}