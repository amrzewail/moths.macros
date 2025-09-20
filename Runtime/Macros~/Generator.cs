using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Moths.Macros
{
    [Generator]
    public class MacrosGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            Compilation compilation = context.Compilation;

            List<ClassDeclarationSyntax> classes = new();
            Dictionary<SyntaxTree, List<DirectiveTriviaSyntax>> directives = new();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                classes.AddRange(root.DescendantNodes()
                                  .OfType<ClassDeclarationSyntax>());

                if (!directives.ContainsKey(syntaxTree)) directives[syntaxTree] = new List<DirectiveTriviaSyntax>();
                var dirs = directives[syntaxTree];

                directives[syntaxTree].AddRange(root.DescendantTrivia()
                    .Where(t => t.IsDirective)                // only directives
                    .Select(t => t.GetStructure())            // get structured node
                    .OfType<DirectiveTriviaSyntax>()          // cast
                    .Where(d => d.ToFullString().Contains("#pragma Macro")));
            }

            Dictionary<string, (Macro, SyntaxList<UsingDirectiveSyntax>)> macros = new Dictionary<string, (Macro, SyntaxList<UsingDirectiveSyntax>)>();

            foreach (var cls in classes)
            {
                if (!HasAttribute(cls, "Macro")) continue;

                macros.Add(cls.Identifier.Text, (new Macro(cls), GetUsings(cls)));
            }

            List<(Pragma pragma, ClassDeclarationSyntax cls)> pragmas = new();

            foreach (var tree in directives)
            {
                var syntaxNode = tree.Key.GetRoot();

                foreach(var dir in tree.Value)
                {
                    var directive = dir;
                    var directiveText = directive.ToFullString();

                    if (IsInsideClass(dir, syntaxNode, out var cls))
                    {
                        pragmas.Add((new Pragma(directiveText), cls));
                        continue;
                    }
                    pragmas.Add((new Pragma(directiveText), null));
                }
            }

            Protection protection = Protection.None;
            Binding binding = Binding.Member;
            Mutability mutability = Mutability.Mutable;

            foreach (var p in pragmas)
            {
                var pragma = p.pragma;
                var cls = p.cls;

                if (!macros.ContainsKey(pragma.Name)) continue;

                SyntaxList<UsingDirectiveSyntax> usings;

                if (cls != null)
                {
                    usings = GetUsings(cls);

                    for (int j = 0; j < cls.Modifiers.Count; j++)
                    {
                        var m = cls.Modifiers[j];

                        switch (m.Kind())
                        {
                            case SyntaxKind.ReadOnlyKeyword:
                                mutability = Mutability.Readonly;
                                break;
                        }
                    }
                }

                (Macro macro, SyntaxList<UsingDirectiveSyntax> macroUsings) = macros[pragma.Name];

                Script script = new Script("");

                foreach (var us in usings)
                {
                    script.AddLine(us.ToFullString());
                }

                foreach (var us in macroUsings)
                {
                    script.AddLine(us.ToFullString());
                }

                string namespaces = cls != null ? GetFullNamespace(cls, context) : "";

                if (!string.IsNullOrEmpty(namespaces)) script.AddLine($"namespace {namespaces}\n{{");

                int parentClassesCount = 0;
                string parentClasses = cls != null ? GetParentClasses(cls, out parentClassesCount) : "";

                if (!string.IsNullOrEmpty(parentClasses)) script.AddLine(parentClasses);

                if (cls != null)
                {
                    script.AddClass(protection, binding, Modifier.Partial, mutability, cls.Identifier.Text);
                    script.Body.AddLine(macro.Generate(pragma.Arguments));
                }
                else
                {
                    script.AddLine(macro.Generate(pragma.Arguments));
                    script.ClearBody();
                }

                script.AddLine($"//{System.DateTime.Now.ToString()}");

                while (parentClassesCount-- > 0) script.Body.AddClosingBrace();

                if (!string.IsNullOrEmpty(namespaces)) script.Body.AddClosingBrace();

                string sourceName = $"{(!string.IsNullOrEmpty(namespaces) ? namespaces + "." : "")}{(cls != null ? cls.Identifier.Text + "." : "")}";

                context.AddSource($"{sourceName}{pragma.Name}.{pragma.GetHashCode()}", SourceText.From(script, Encoding.UTF8));
            }
        }

        private bool IsInsideClass(DirectiveTriviaSyntax directive, SyntaxNode root, out ClassDeclarationSyntax cls)
        {
            var position = directive.SpanStart;

            var classNode = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Span.Contains(position));

            cls = classNode;

            return cls != null;
        }

        private string GetParentClasses(ClassDeclarationSyntax classNode, out int count)
        {
            var classes = new System.Collections.Generic.List<ClassDeclarationSyntax>();

            // Collect parent classes up to the top
            var current = classNode.Parent as ClassDeclarationSyntax;
            while (current != null)
            {
                classes.Insert(0, current); // Insert at start for outermost first
                current = current.Parent as ClassDeclarationSyntax;
            }

            // Build nested class structure
            string result = "";
            count = 0;
            foreach (var cls in classes)
            {
                string modifiers = cls.Modifiers.ToFullString().Trim();
                string partial = modifiers.Contains("partial") ? "partial " : "";
                result += new string(' ', count * 4) + $"{partial}class {cls.Identifier.Text} {{\n";
                count++;
            }

            //// Close braces
            //for (int i = classes.Count - 1; i >= 0; i--)
            //{
            //    result += new string(' ', i * 4) + "}\n";
            //}

            return result;
        }

        private string GetFullNamespace(ClassDeclarationSyntax cls, GeneratorExecutionContext context)
        {
            var model = context.Compilation.GetSemanticModel(cls.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(cls);
            var nms = symbol?.ContainingNamespace?.ToDisplayString();
            if (nms == "<global namespace>") return "";
            return nms;
        }

        private SyntaxList<UsingDirectiveSyntax> GetUsings(SyntaxNode cls)
        {
            SyntaxList<UsingDirectiveSyntax> usings = default;

            // Climb up the parent chain until we hit a namespace or the root
            SyntaxNode? node = cls.Parent;
            while (node != null)
            {
                if (node is NamespaceDeclarationSyntax ns)
                    usings = usings.AddRange(ns.Usings);
                if (node is FileScopedNamespaceDeclarationSyntax fns)
                    usings = usings.AddRange(fns.Usings);
                if (node is CompilationUnitSyntax cu)
                    usings = usings.AddRange(cu.Usings);

                node = node.Parent;
            }

            return usings;
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
