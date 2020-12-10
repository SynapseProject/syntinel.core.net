using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Syntinel.Version
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = args[0];

            int major = 0;
            int minor = 1;
            int build = (DateTime.Today.Year - 2000) * 1000 + DateTime.Today.DayOfYear;
            int revision = 0;

            string[] versions = { $"{major}.{minor}.{build}.{revision}" };
            string[] patterns = {
                "AssemblyVersion\\(\"(.*)\"\\)",
                "AssemblyFileVersion\\(\"(.*)\"\\)"
            };

            foreach (string arg in args)
                Console.WriteLine(">>>>> " + arg);

            RegexReplace(filename, patterns, versions);
        }

        static void RegexReplace(string file, string pattern, string value)
        {
            RegexReplace(file, new string[] { pattern }, new string[] { value });
        }


        static void RegexReplace(string file, string[] patterns, string[] values)
        {
            String[] content = File.ReadAllLines(file);

            for(int i = 0; i<content.Length; i++)
            {
                string line = content[i];
                foreach (string pattern in patterns)
                {
                    Regex r = new Regex(pattern);
                    Match match = r.Match(line);
                    if (match.Success)
                    {
                        Console.WriteLine(line);
                        for (int j = 1; j < match.Groups.Count; j++)
                            if (values.Length > (j - 1))
                            {
                                string matchValue = match.Groups[j].Value;
                                line = line.Replace(matchValue, values[j - 1]);
                            }
                        content[i] = line;
                    }
                }
            }

            File.WriteAllLines(file, content);
        }
    }
}
