using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Moths.Macros
{
    internal struct Macro
    {
        private static MacroExpressionRewriter ExpressionRewriter = new MacroExpressionRewriter();

        private StringBuilder _text;
        private StringBuilder _generated;

        private List<string> _args;

        public IReadOnlyList<string> Args => _args;

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

            var body = cls.SyntaxTree.GetText().ToString(Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(cls.OpenBraceToken.Span.End, cls.CloseBraceToken.Span.Start));

            string errors = "";
            
            try
            {
                var root = CSharpSyntaxTree.ParseText(body).GetRoot();
                var newRoot = ExpressionRewriter.Visit(root);
                body = newRoot.ToFullString();
            }
            catch(System.Exception ex)
            {
                errors += $"/*{ex.ToString()}*/";
            }

            body += errors;

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
                var arg = _args[i];
                if (arg.StartsWith("type:")) arg = arg.Substring("type:".Length);
                _generated.Replace(arg, args[i]);
            }

            return _generated.ToString();
        }
    }
}
