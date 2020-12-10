using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Syntinel.Version
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectDir = args[0];
            string projectName = Path.GetFileName(projectDir);  // Since ${ProjectDir} Doesn't Include Trailing Slash

            Console.WriteLine($">>> Project Dir  : {projectDir}");
            Console.WriteLine($">>> Project Name : {projectName}");

            int major = 0;
            int minor = 1;
            int build = (DateTime.Today.Year - 2000) * 1000 + DateTime.Today.DayOfYear;
            int revision = 0;
            string version = $"{major}.{minor}.{build}.{revision}";

            // Update AssemblyInfo.cs
            string[] versions = { version };
            string[] patterns = {
                "AssemblyVersion\\(\"(.*)\"\\)",
                "AssemblyFileVersion\\(\"(.*)\"\\)"
            };

            string filename = projectDir + "/Properties/AssemblyInfo.cs";
            Console.WriteLine($">>> AssemblyFile : {filename}");
            RegexReplace(filename, patterns, versions);

            // Update Cloud Formation Templates
            if ("Syntinel.Aws".Equals(projectName))
            {
                string templatesDir = $"{projectDir}/cf-templates";
                foreach (string template in Directory.GetFiles(templatesDir, "*.yaml"))
                    Console.WriteLine($">>> Template     : {template}");

                templatesDir = $"{projectDir}/cf-templates/stacks";
                foreach (string template in Directory.GetFiles(templatesDir, "*.yaml"))
                    Console.WriteLine($">>> Template     : {template}");
            }

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
