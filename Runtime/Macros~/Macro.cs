using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Moths.Macros
{
    internal struct Macro
    {
        private static Regex FindMemberMethodRegex = new Regex(@"Macro\.Expression(?:<[^>]+>)?\(\s*[@$]?""([^""]+)""\s*\).Call");
        private static Regex FindMemberRegex = new Regex(@"Macro\.Expression(?:<[^>]+>)?\(\s*[@$]?""([^""]+)""\s*\)");

        private StringBuilder _text;
        private StringBuilder _generated;

        private List<string> _args;

        public Macro(ClassDeclarationSyntax cls)
        {
            _args = new List<string>();

            foreach (var attr in cls.AttributeLists.SelectMany(al => al.Attributes))
            {
                if (attr.Name.ToString() == "Macro")
                {
                    if (attr.ArgumentList == null) continue;

                    foreach (var arg in attr.ArgumentList.Arguments)
                    {
                        _args.Add(arg.GetText().ToString().TrimStart('"').TrimEnd('"'));
                    }
                }
            }

            var text = cls.ToFullString();

            var body = text.Substring(text.IndexOf("{") + 1, text.LastIndexOf("}") - text.IndexOf("{") - 1);

            body = FindAndReplaceMembers(body);

            _text = new StringBuilder(body);
            _generated = new StringBuilder();
        }

        public string Generate(IReadOnlyList<string> args)
        {
            if (_args.Count != args.Count) return "";

            _generated.Clear();
            _generated.Append(_text);

            for (int i = 0; i < _args.Count; i++)
            {
                _generated.Replace(_args[i], args[i]);
            }

            return _generated.ToString();
        }

        private string FindAndReplaceMembers(string macro)
        {
            string temp = macro;
            temp = FindMemberMethodRegex.Replace(temp, m => m.Groups[1].Value);
            temp = FindMemberRegex.Replace(temp, m => m.Groups[1].Value);
            return temp;
        }
    }
}
