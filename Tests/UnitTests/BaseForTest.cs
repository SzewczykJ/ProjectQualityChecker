using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ProjectQualityChecker.Data;
using ProjectQualityChecker.Models;

namespace Tests.UnitTests
{
    public abstract class BaseForTest
    {
        public AppDbContext GetDbContext()
        {
            DbContextOptions<AppDbContext> options;
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("testDB", new InMemoryDatabaseRoot());
            options = builder.Options;
            var appDbContext = new AppDbContext(options);
            appDbContext.Database.EnsureDeleted();
            appDbContext.Database.EnsureCreated();

            return appDbContext;
        }

        public Task<Project> GivenCreateProject()
        {
            return Task.FromResult(new Project
            {
                Key = "projectKeyTest",
                Name = "projectNameTest"
            });
        }

        public Task<ProjectToken> GivenCreateToken()
        {
            return Task.FromResult(new ProjectToken
            {
                Token = "projectTokenTest"
            });
        }

        public HttpClient GivenHttpClient()
        {
            var credentials = Encoding.ASCII.GetBytes("admin:adminq");
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:8000/api/");
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            return httpClient;
        }

        public Task<ProjectTree> GivenProjectTree()
        {
            var components = new List<Component>();

            components.Add(new Component
            {
                Qualifier = "FIL",
                Key = "projectKeyTest1",
                Path = "fileName.xml"
            });
            components.Add(new Component
            {
                Qualifier = "FIL",
                Key = "projectKeyTest2",
                Path = "differentFileName.cpp"
            });

            var projectTree = new ProjectTree
            {
                Components = components
            };

            return Task.FromResult(projectTree);
        }

        public Task<Root> GivenMetrics()
        {
            var measures = new List<Measure>();

            measures.Add(new Measure
            {
                Metric = "complexity",
                Value = "1"
            });
            measures.Add(new Measure
            {
                Metric = "cognitive_complexity",
                Value = "2"
            });
            measures.Add(new Measure
            {
                Metric = "duplicated_lines",
                Value = "3"
            });
            measures.Add(new Measure
            {
                Metric = "code_smells",
                Value = "4"
            });
            measures.Add(new Measure
            {
                Metric = "new_code_smells",
                Value = "8"
            });
            measures.Add(new Measure
            {
                Metric = "comment_lines",
                Value = "5"
            });
            measures.Add(new Measure
            {
                Metric = "comment_lines_density",
                Value = "5.5"
            });
            measures.Add(new Measure
            {
                Metric = "ncloc",
                Value = "6"
            });
            measures.Add(new Measure
            {
                Metric = "statements",
                Value = "7"
            });
            measures.Add(new Measure
            {
                Metric = "branch_coverage",
                Value = "7.5"
            });
            measures.Add(new Measure
            {
                Metric = "line_coverage",
                Value = "7.7"
            });

            var root = new Root
            {
                Component = new File
                {
                    Name = "fileName",
                    Path = "fileName.xml",
                    Qualifier = "FIL",
                    Key = "projectKeyTest1",
                    Measures = measures
                }
            };

            return Task.FromResult(root);
        }
    }
}