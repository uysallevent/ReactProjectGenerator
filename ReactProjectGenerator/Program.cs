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
                ProjectGenerationManagement();
                NpmPackageManagement();
                FolderManagement();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        private static void ProjectGenerationManagement()
        {
            var projectGenerationScript = ScriptReaderHelper.ReadScriptFile("CreateReactAppScript.txt");
            RunScript(
           projectGenerationScript,
           AppDomain.CurrentDomain.BaseDirectory,
           "Project generation process has been started. Please wait !!!");
            projectPath = projectPath ?? $"{AppDomain.CurrentDomain.BaseDirectory}{projectGenerationScript.Split(" ").Last()}";
        }

        private static void NpmPackageManagement()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Scripts\\NpmPackageList.txt";
            var content = File.ReadAllLines(path);
            if (content == null || content.Length == 0)
            {
                Console.WriteLine("There is no npm packages found for installing");
                return;
            }

            RunScript(
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
                    if (!Directory.Exists(fileInfo.DirectoryName))
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

        private static void RunScript(string script, string executePath, string waitingMessage)
        {
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();
                runspace.SessionStateProxy.Path.SetLocation(executePath);
                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.AddScript(script);
                    pipeline.InvokeAsync();

                    Console.WriteLine(waitingMessage);
                    Console.WriteLine($">>>>>>{script}");

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
