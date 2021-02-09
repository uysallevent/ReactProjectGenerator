using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnivesalProjectGenerator.Enums;

namespace UniversalProjectGenerator.Helpers
{
    public static class ScriptReaderHelper
    {

        public static string ReadScriptFile(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException($"There is no file on {scriptPath} path");
            }
            var content = File.ReadAllText(scriptPath);
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
                    while (true)
                    {
                        Console.WriteLine($"Please enter a {schemaName.Replace(":{", "").Replace("}:", "")} :");
                        var inputValue = Console.ReadLine();
                        if (!string.IsNullOrEmpty(inputValue))
                        {
                            content = content.Replace(schemaName, inputValue);
                            break;
                        }
                        else
                        {
                            Console.SetCursorPosition($"Please enter a {schemaName.Replace(":{", "").Replace("}:", "")} :".Length, 0);
                        }
                    }

                }
                return content;
            }
            return null;
        }
    }
}
