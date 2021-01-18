using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectQualityChecker.Data.Database;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Models;
using Commit = ProjectQualityChecker.Data.Database.Commit;
using File = ProjectQualityChecker.Data.Database.File;

namespace ProjectQualityChecker.Services
{
    public class SonarQubeService
    {
        private readonly IFileDetailRepo _fileDetailRepo;
        private readonly IFileRepo _fileRepo;
        private readonly ILanguageRepo _languageRepo;
        private readonly SonarQubeClient _sonarQubeClient;

        public SonarQubeService(SonarQubeClient sonarQubeClient, IFileRepo fileRepo,
            IFileDetailRepo fileDetailRepo, ILanguageRepo languageRepo)
        {
            _sonarQubeClient = sonarQubeClient;
            _fileRepo = fileRepo;
            _languageRepo = languageRepo;
            _fileDetailRepo = fileDetailRepo;
        }

        public async Task SaveDataFromRepositoryAsync(string projectName, Commit sonarCommit,
            Dictionary<string, CommitChanges> commitChanges)
        {
            Thread.Sleep(5000);
            var projectTree = await _sonarQubeClient.GetProjectTree(projectName);

            await SaveFileMetricAsync(projectTree, sonarCommit, commitChanges);
        }

        private async Task SaveFileMetricAsync(ProjectTree projectTree, Commit sonarCommit,
            Dictionary<string, CommitChanges> commitChanges)
        {
            foreach (var component in projectTree.Components)
                if (component.Path != null && commitChanges.ContainsKey(component.Path))
                    if (component.Qualifier.Equals("FIL") || component.Qualifier.Equals("UTS"))
                    {
                        var root = await _sonarQubeClient.GetMetricsForFile(component.Key);
                        var metric = MappMetric(root.Component.Measures);
                        var fileDetail = _fileDetailRepo.FindByPath(component.Path);

                        if (fileDetail == null)
                            fileDetail = new FileDetail
                            {
                                Name = component.Name,
                                FullPath = component.Path,
                                Language = GetLanguage(component.Language)
                            };

                        var file = new File
                        {
                            SHA = commitChanges.GetValueOrDefault(component.Path).SHA,
                            FileDetail = fileDetail,
                            Metric = metric,
                            Commit = sonarCommit,
                            Status = commitChanges.GetValueOrDefault(component.Path).Status.ToString()
                        };

                        _fileRepo.Add(file);
                    }
        }

        private Metric MappMetric(List<Measure> measures)
        {
            var metric = new Metric();
            metric.Date = DateTime.UtcNow;

            foreach (var measure in measures)
                switch (measure.Metric)
                {
                    case "complexity":
                        metric.Complexity = int.Parse(measure.Value);
                        break;
                    case "cognitive_complexity":
                        metric.CognitiveComplexity = int.Parse(measure.Value);
                        break;
                    case "duplicated_lines":
                        metric.DuplicatedLines = int.Parse(measure.Value);
                        break;
                    case "code_smells":
                        metric.CodeSmells = int.Parse(measure.Value);
                        break;
                    case "new_code_smells":
                       metric.NewCodeSmells = measure.Value == null ? (int?) null : int.Parse(measure.Value);
                       break;
                   case "comment_lines":
                        metric.CommentLines = int.Parse(measure.Value);
                        break;
                    case "comment_lines_density":
                        metric.CommentLinesDensity = double.Parse(measure.Value, CultureInfo.InvariantCulture);
                        break;
                    case "ncloc":
                        metric.Ncloc = int.Parse(measure.Value);
                        break;
                    case "statements":
                        metric.Statements = int.Parse(measure.Value);
                        break;
                    case "branch_coverage":
                        metric.BranchCoverage = double.Parse(measure.Value, CultureInfo.InvariantCulture);
                        break;
                    case "line_coverage":
                        metric.LineCoverage = double.Parse(measure.Value, CultureInfo.InvariantCulture);
                        break;
                }

            return metric;
        }

        private Language GetLanguage(string sonarLanguage)
        {
            switch (sonarLanguage)
            {
                case "java":
                    return _languageRepo.FindByName("Java");
                case "cs":
                    return _languageRepo.FindByName("C#");
                case "php":
                    return _languageRepo.FindByName("PHP");
                case "js":
                    return _languageRepo.FindByName("JavaScript");
                case "ts":
                    return _languageRepo.FindByName("TypeScript");
                case "kotlin":
                    return _languageRepo.FindByName("Kotlin");
                case "ruby":
                    return _languageRepo.FindByName("Ruby");
                case "go":
                    return _languageRepo.FindByName("Go");
                case "scala":
                    return _languageRepo.FindByName("Scala");
                case "flex":
                    return _languageRepo.FindByName("Flex");
                case "py":
                    return _languageRepo.FindByName("Python");
                case "web":
                    return _languageRepo.FindByName("HTML");
                case "css":
                    return _languageRepo.FindByName("CSS");
                case "xml":
                    return _languageRepo.FindByName("XML");
                case "vbnet":
                    return _languageRepo.FindByName("VB.NET");
                default:
                    return _languageRepo.FindByName("Other");
            }
        }
    }
}