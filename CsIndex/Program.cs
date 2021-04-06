using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace CsIndex
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();

                var files = GetFiles(dir, "*.cs");

                var regex = new Regex("[a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                foreach (var f in files)
                {
                    var arr = File.ReadAllLines(f);
                    var matches = arr.Select((x, i) => new { Matches = regex.Matches(x), Linenumber = i + 1 } );
                }
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine($"{progname} Error: {ex.Message}");
            }

        }

        private static List<string> GetFiles(string startDir, string pattern)
        {
            var result = new List<string>();
            var stack = new Stack<string>();

            stack.Push(startDir);
            while (stack.Any())
            {
                var currentDir = stack.Pop();
                var files = Directory.GetFiles(currentDir, pattern);
                result.AddRange(files);
                result.RemoveAll(x => x.IndexOf("temporary_generated", StringComparison.OrdinalIgnoreCase) >= 0);
                foreach (var dir in Directory.GetDirectories(currentDir))
                {
                    var name = new DirectoryInfo(dir).Name;
                    if (dir.First() != '.')
                    {
                        stack.Push(dir);
                    }
                }
            }

            return result;
        }
    }
}
