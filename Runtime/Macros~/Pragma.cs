using System;
using System.Text.RegularExpressions;


namespace Analyzers
{
    public struct Pragma
    {
        public string Name { get; }
        public string[] Arguments { get; }

        public Pragma(string line)
        {
            var match = Regex.Match(line, @"^\s*#pragma\s+(\w+)\s*\(([^)]*)\)");
            if (!match.Success) return;

            var name = match.Groups[1].Value;
            var argsRaw = match.Groups[2].Value;

            // split arguments by comma, trim spaces
            var args = string.IsNullOrWhiteSpace(argsRaw)
                ? Array.Empty<string>()
                : Array.ConvertAll(argsRaw.Split(','), a => a.Trim());

            Name = name;
            Arguments = args;
        }

        public override string ToString()
        {
            return $"#pragma {Name}({string.Join(", ", Arguments)})";
        }
    }
}
