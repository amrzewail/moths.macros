using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Moths.Macros
{
    public struct Pragma
    {
        public string Name { get; }
        public string[] Arguments { get; }

        public Pragma(string line)
        {
            var match = Regex.Match(line, @"^\s*#pragma Macro\s+(\w+)(?:\s*\((.*)\))?");
            if (!match.Success) return;

            var name = match.Groups[1].Value;
            var argsRaw = match.Groups[2].Value; // may be "" if no parentheses

            var args = string.IsNullOrWhiteSpace(argsRaw)
                ? Array.Empty<string>()
                : argsRaw.Split(',')
                         .Select(a => a.Trim())
                         .Where(a => a.Length > 0)
                         .ToArray();

            for (int i = 0; i < args.Length; i++) args[i] = args[i].Replace(@"|", ",");

            Name = name;
            Arguments = args;
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
