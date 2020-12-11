using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Syntinel.Version
{
    public class Program
    {
        public static void Main(string[] args)
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
                    YamlReplace(template, "Outputs.Version.Value", version);

                templatesDir = $"{projectDir}/cf-templates/stacks";
                foreach (string template in Directory.GetFiles(templatesDir, "*.yaml"))
                    YamlReplace(template, "Outputs.Version.Value", version);
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
                    Match match = Regex.Match(line, pattern);
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

        static void YamlReplace(string file, string path, string value)
        {
            string[] parts = path.Split('.');
            string[] content = File.ReadAllLines(file);

            int partIndex = 0;
            int indention = -1;
            bool found = false;

            for (int i=0; i<content.Length; i++)
            {
                string line = content[i];
                string pattern = $"^(\\s*){parts[partIndex]}";
                Match match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    string spaces = match.Groups[1].Value;
                    if (partIndex == parts.Length - 1)
                    {
                        found = true;
                        content[i] = $"{spaces}{parts[partIndex]}: {value}";
                        break;
                    }
                    else if (spaces.Length > indention)
                    {
                        partIndex++;
                        indention = spaces.Length;
                    }
                }
            }

            if (found)
                File.WriteAllLines(file, content);

        }
    }
}
