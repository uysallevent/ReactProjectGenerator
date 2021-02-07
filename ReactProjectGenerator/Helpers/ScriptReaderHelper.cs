using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactProjectGenerator.Helpers
{
    public static class ScriptReaderHelper
    {
        public static async Task<string> ReadScriptFile(string scriptFileName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Scripts\\{scriptFileName}";
            var content = await File.ReadAllTextAsync(path);
            var script = await InputChecker(content);

            return script;
        }

        private static async Task<string> InputChecker(string content)
        {
            if (content.Contains(":{") || content.Contains("}:"))
            {
                var countInput = content.Split(":{").Length - 1;
                for (int i = 0; i < countInput; i++)
                {
                    var schemaName = content.Substring(content.IndexOf(":{"), (content.IndexOf("}:") - content.IndexOf(":{")) + 2);

                    await Console.Out.WriteAsync($"Please enter a {schemaName.Replace(":{", "").Replace("}:", "")} :");
                    var inputValue = Console.ReadLine();
                    content = content.Replace(schemaName, inputValue);
                }
                return content;
            }
            return null;
        }
    }
}
