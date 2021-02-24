using UniversalProjectGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using UnivesalProjectGenerator.ProjectTypes.React;

namespace UniversalProjectGenerator
{
    static class Program
    {
        static StringArrayComparer stringArrayComparer;

        static void Main(string[] args)
        {
            try
            {
                stringArrayComparer = new StringArrayComparer();
                MainAsync(args).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        static async Task MainAsync(string[] args)
        {
            await Console.Out.WriteLineAsync(" - - - - - - - - - - - - - - - - - - ");
            await Console.Out.WriteLineAsync("Algomedi Universal Project Generator");
            await Console.Out.WriteLineAsync(" - - - - - - - - - - - - - - - - - - ");
            await Console.Out.WriteLineAsync();


            await WaitCommandAsync();
        }

        private static async Task WaitCommandAsync()
        {
            while (true)
            {
                await Console.Out.WriteLineAsync("").ConfigureAwait(false);
                await Console.Out.WriteLineAsync("Waiting your command :").ConfigureAwait(false);
                await ExecuteCommandAsync(Console.ReadLine().Split(' ').Select(p => p.ToLowerInvariant()).ToArray());
            }
        }

        private static async Task ExecuteCommandAsync(string[] command)
        {
            foreach (var item in GeneralCommandList())
            {
                if (stringArrayComparer.Equals(item.Key, command))
                {
                    await item.Value.Invoke("testtt");
                    return;
                }
            }
            await Console.Out.WriteLineAsync($"There is no command found as {string.Join(" ", command)}");
        }

        private static Dictionary<string[], Func<string, Task>> GeneralCommandList()
        {
            return new Dictionary<string[], Func<string, Task>>()
            {
                 { new string[]{"help"}, async (c) =>{ await Console.Out.WriteLineAsync("Help contents");}},
                 { new string[]{"react", "generate" }, async (c) =>{await ReactGeneration.Generate(); }},
                 { new string[]{ "react", "test" }, async (c) =>{ await Console.Out.WriteLineAsync("Test2 ignated"); }}
            };
        }







    }
}
