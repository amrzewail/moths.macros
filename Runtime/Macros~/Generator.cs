using Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExampleSourceGenerator
{
    [Generator]
    public class ExampleSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {

            Compilation compilation = context.Compilation;

            List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();

            // Inspect syntax trees
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                string sourceText = syntaxTree.ToString();
                string filePath = syntaxTree.FilePath;

                // Example: log class names
                var root = syntaxTree.GetRoot();
                classes.AddRange(root.DescendantNodes()
                                  .OfType<ClassDeclarationSyntax>());
            }

            Dictionary<string, Macro> macros = new Dictionary<string, Macro>();

            foreach (var cls in classes)
            {
                if (!HasAttribute(cls, "Macro")) continue;

                macros.Add(cls.Identifier.Text, new Macro(cls));
            }

            foreach (var cls in classes)
            {
                var lines = cls.ToFullString().Split('\n');
                SyntaxList<UsingDirectiveSyntax> usings = default;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (!line.Trim().StartsWith("#pragma")) continue;

                    Pragma pragma = new Pragma(line);

                    if (!macros.ContainsKey(pragma.Name)) continue;

                    if (usings == default) usings = GetUsings(cls);

                    Script script = new Script("");

                    foreach (var us in usings)
                    {
                        script.AddLine(us.ToFullString());
                    }

                    script.AddClass(Protection.Public, Binding.Member, Modifier.Partial, Mutability.Mutable, cls.Identifier.Text);

                    script.Body.AddLine(macros[pragma.Name].Generate(pragma.Arguments));

                    script.Body.AddComment(System.DateTime.Now.ToString());

                    context.AddSource($"{cls.Identifier.Text}_{i}", SourceText.From(script, Encoding.UTF8));
                }
            }
        }

        private SyntaxList<UsingDirectiveSyntax> GetUsings(SyntaxNode cls)
        {
            // Climb up the parent chain until we hit a namespace or the root
            SyntaxNode? node = cls.Parent;
            while (node != null)
            {
                if (node is NamespaceDeclarationSyntax ns)
                    return ns.Usings;
                if (node is FileScopedNamespaceDeclarationSyntax fns)
                    return fns.Usings;
                if (node is CompilationUnitSyntax cu)
                    return cu.Usings;

                node = node.Parent;
            }

            return default;
        }

        private bool HasAttribute(ClassDeclarationSyntax cls, string attributeName)
        {
            return cls.AttributeLists
                      .SelectMany(al => al.Attributes)
                      .Any(attr => attr.Name.ToString() == attributeName ||
                                   attr.Name.ToString() == attributeName + "Attribute");
        }

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
