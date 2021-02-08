using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using ReactProjectGenerator.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactProjectGenerator
{
    class Program
    {
        static string projectPath = null;

        static void Main(string[] args)
        {
            try
            {
                MainAsync(args).Wait();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        static async Task MainAsync(string[] args)
        {
            ProjectGenerationManagement().Wait();
            NpmPackageManagement().Wait();
            FolderManagement().Wait();
        }

        private static async Task ProjectGenerationManagement()
        {
            var projectGenerationScript = await ScriptReaderHelper.ReadScriptFile("CreateReactAppScript.txt");
            await RunScript(
            projectGenerationScript,
            AppDomain.CurrentDomain.BaseDirectory,
            "Project generation process has been started. Please wait !!!");
            projectPath = projectPath ?? $"{AppDomain.CurrentDomain.BaseDirectory}{projectGenerationScript.Split(" ").Last()}";
        }

        private static async Task NpmPackageManagement()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Scripts\\NpmPackageList.txt";
            var content = await File.ReadAllLinesAsync(path);
            if (content == null || content.Length == 0)
            {
                await Console.Out.WriteLineAsync("There is no npm packages found for installing");
                return;
            }

            await RunScript(
            $"npm install { string.Join(" ", content)}",
            projectPath,
            "Npm package installation process has been started. Please wait !!!");
        }

        private static async Task FolderManagement()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Scripts\\FolderAndFiles.txt";
            var content = await File.ReadAllLinesAsync(path);
            if (content == null || content.Length == 0)
            {
                await Console.Out.WriteLineAsync("There is no folder or file found for creating");
                return;
            }

            var srcPath = $"{projectPath}\\src";
            foreach (var item in content)
            {
                if (item.Split(".").Last().ToLower() == "js" || item.Split(".").Last().ToLower() == "jsx")
                {
                    var fileInfo = new FileInfo($"{srcPath}\\{item}");
                    if(!Directory.Exists(fileInfo.DirectoryName))
                    {
                        Directory.CreateDirectory(fileInfo.DirectoryName);
                    }

                    File.Create($"{srcPath}\\{item}");
                }
                else
                {
                    Directory.CreateDirectory($"{srcPath}\\{item}");
                }
            }
        }

        private static async Task RunScript(string script, string executePath, string waitingMessage)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                runspace.SessionStateProxy.Path.SetLocation(executePath);
                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.AddScript(script);
                    pipeline.InvokeAsync();

                    await Console.Out.WriteLineAsync(waitingMessage);
                    await Console.Out.WriteLineAsync($">>>>>>{script}");

                    while (pipeline.PipelineStateInfo.State == PipelineState.Running || pipeline.PipelineStateInfo.State == PipelineState.Stopping)
                    {
                        ConsoleSpinnerHelper.Turn();
                    }

                    foreach (object item in pipeline.Error.ReadToEnd())
                    {
                        if (item != null)
                            Console.WriteLine(item.ToString());
                    }
                }
                runspace.Close();
            }
        }

    }
}
