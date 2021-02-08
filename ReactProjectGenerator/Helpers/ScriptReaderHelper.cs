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
        public static string ReadScriptFile(string scriptFileName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}Scripts\\{scriptFileName}";
            var content = File.ReadAllText(path);
            var script = InputChecker(content);
            return script;
        }

        private static string InputChecker(string content)
        {
            if (content.Contains(":{") || content.Contains("}:"))
            {
                var countInput = content.Split(":{").Length - 1;
                for (int i = 0; i < countInput; i++)
                {
                    var schemaName = content.Substring(content.IndexOf(":{"), (content.IndexOf("}:") - content.IndexOf(":{")) + 2);

                    Console.WriteLine($"Please enter a {schemaName.Replace(":{", "").Replace("}:", "")} :");

                repeat:
                    var inputValue = Console.ReadLine();
                    if (!string.IsNullOrEmpty(inputValue))
                    {
                        content = content.Replace(schemaName, inputValue);
                    }
                    else
                    {
                        Console.SetCursorPosition($"Please enter a {schemaName.Replace(":{", "").Replace("}:", "")} :".Length, 0);
                        goto repeat;
                    }
                }
                return content;
            }
            return null;
        }
    }
}
