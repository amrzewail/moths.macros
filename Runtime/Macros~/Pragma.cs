using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Moths.Macros
{
    public struct Pragma
    {
        public string Name { get; }
        public string[] Arguments { get; }

        public Pragma(string line)
        {
            var match = Regex.Match(line, @"^\s*#pragma Macro\s+(\w+)\s*\(([^)]*)\)");
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
        public override int GetHashCode()
        {
            int hashCode = -1017281739;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(Arguments);
            return hashCode;
        }
    }
}
