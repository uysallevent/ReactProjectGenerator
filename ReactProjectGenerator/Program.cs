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
            var script = await ScriptReaderHelper.ReadScriptFile("CreateReactAppScript.txt");
            await RunScript(script, "Project generation process has been started. Please wait !!!", "Project has been generated");

            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Scripts\\NpmPackageList.txt";
            var content = await File.ReadAllLinesAsync(path);
            await RunScript(
                $"cd {AppDomain.CurrentDomain.BaseDirectory}\\test" +
                $"npm install { string.Join(" ", content)}", "Npm package installation process has been started", "Npm packages installation completed");

        }


        private static async Task RunScript(string script, string waitingMessage, string resultMessage)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(script);

                IAsyncResult result = PowerShellInstance.BeginInvoke();
                await Console.Out.WriteLineAsync(waitingMessage);
                while (result.IsCompleted == false)
                {
                    ConsoleSpinnerHelper.Turn();
                }
                await Console.Out.WriteLineAsync(resultMessage);
                PowerShellInstance.Dispose();
            }
        }
    }
}
