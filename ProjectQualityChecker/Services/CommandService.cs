using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ProjectQualityChecker.Services
{
    public class CommandService
    {
        private readonly string SONAR_URL = "http://sonarqube:9000";
      
        public int CheckoutCommit(string commitSHA, string workingDirectory)
        {
            return Execute("git", $"checkout -f {commitSHA}", workingDirectory);
        }


        public int BuildMavenProject(string workingDirectory)
        {
            return Execute("mvn", @"clean install -DskipTests", workingDirectory);
        }

        public int ScanMavenProject(string projectKey, string loginToken, string workingDirectory, string changedFiles)
        {
            if (changedFiles == null)
            {
                return Execute("mvn",
                    $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken}",
                    workingDirectory);
            }

            return Execute("mvn",
                $@"sonar:sonar -Dsonar.projectKey={projectKey} -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken} -Dsonar.inclusions={changedFiles}",
                workingDirectory);
        }


        public int DotnetScanner(string projectKey, string loginToken, string workingDirectory, string changedFiles)
        {
            if (changedFiles == null)
                return Execute("/bin/bash",
                    $@"-c ""dotnet sonarscanner begin /k:""{projectKey}"" /d:sonar.host.url={SONAR_URL} /d:sonar.login=""{loginToken}"" ",
                    workingDirectory);

            return Execute("/bin/bash",
                @"-c ""dotnet sonarscanner begin /k:""{projectKey}"" /d:sonar.host.url={SONAR_URL} /d:sonar.login=""{loginToken}"" /d:sonar.inclusions=""{changedFiles}""""",
                workingDirectory);
        }

        public int DotnetRestore(string workingDirectory)
        {
            return Execute("/bin/bash", @"-c ""dotnet restore""", workingDirectory);
        }

        public int RebuildDotnetProject(string workingDirectory)
        {
            return Execute("/bin/bash", @"-c ""dotnet build""", workingDirectory);
        }

        public int EndDotnetScanner(string loginToken, string workingDirectory)
        {
            return Execute("/bin/bash", $@"-c ""dotnet sonarscanner end /d:sonar.login=""{loginToken}""""",
                workingDirectory);
        }

        public int ScanOtherType(string projectKey, string loginToken, string workingDirectory, string changedFiles)
        {
            if (changedFiles == null)
                return Execute("/bin/bash",
                    $@"-c ""sonar-scanner -Dsonar.projectKey={projectKey} -Dsonar.sources=. -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken}""",
                    workingDirectory);


            return Execute("/bin/bash",
                $@"-c ""sonar-scanner -Dsonar.projectKey={projectKey} -Dsonar.sources=. -Dsonar.host.url={SONAR_URL} -Dsonar.login={loginToken} -Dsonar.inclusions={changedFiles}""",
                workingDirectory);
        }

        public int Execute(string command, string argument, string workingDirectory)
        {
            var process = new Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = argument;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            var err = process.StandardError.ReadToEnd();
            Console.WriteLine(err);

            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Close();

            return exitCode;
        }
    }
}
