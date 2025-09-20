using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moths.Macros
{
    internal struct Macro
    {
        private string _text;
        private List<string> _args;

        public Macro(ClassDeclarationSyntax cls)
        {
            _args = new List<string>();
            foreach (var attr in cls.AttributeLists.SelectMany(al => al.Attributes))
            {
                if (attr.Name.ToString() == "Macro")
                {
                    foreach (var arg in attr.ArgumentList.Arguments)
                    {
                        _args.Add(arg.GetText().ToString().TrimStart('"').TrimEnd('"'));
                    }
                }
            }

            var text = cls.ToFullString();

            var body = text.Substring(text.IndexOf("{") + 1, text.LastIndexOf("}") - text.IndexOf("{") - 1);

            _text = body;
        }

        public string Generate(IReadOnlyList<string> args)
        {
            if (_args.Count != args.Count) return "";

            string txt = _text;

            for (int i = 0; i < _args.Count; i++)
            {
                txt = txt.Replace($"Macro.Arg(\"{_args[i]}\")", args[i]);
                txt = txt.Replace(_args[i], args[i]);
            }

            return txt;
        }
    }
}
