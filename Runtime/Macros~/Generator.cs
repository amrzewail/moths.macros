using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
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

            List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = syntaxTree.GetRoot();
                classes.AddRange(root.DescendantNodes()
                                  .OfType<ClassDeclarationSyntax>());
            }

            Dictionary<string, (Macro, SyntaxList<UsingDirectiveSyntax>)> macros = new Dictionary<string, (Macro, SyntaxList<UsingDirectiveSyntax>)>();

            foreach (var cls in classes)
            {
                if (!HasAttribute(cls, "Macro")) continue;

                macros.Add(cls.Identifier.Text, (new Macro(cls), GetUsings(cls)));
            }

            foreach (var cls in classes)
            {
                var lines = cls.ToFullString().Split('\n');

                bool didInitialize = false;

                SyntaxList<UsingDirectiveSyntax> usings = default;

                Protection protection = Protection.None;
                Binding binding = Binding.Member;
                Mutability mutability = Mutability.Mutable;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (!line.Trim().StartsWith("#pragma Macro")) continue;

                    Pragma pragma = new Pragma(line);

                    if (!macros.ContainsKey(pragma.Name)) continue;

                    if (!didInitialize)
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

                        didInitialize = true;
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

                    string namespaces = GetFullNamespace(cls, context);

                    if (!string.IsNullOrEmpty(namespaces)) script.AddLine($"namespace {namespaces}\n{{");

                    string parentClasses = GetParentClasses(cls, out int parentClassesCount);

                    if (!string.IsNullOrEmpty(parentClasses)) script.AddLine(parentClasses);

                    script.AddClass(protection, binding, Modifier.Partial, mutability, cls.Identifier.Text);

                    script.Body.AddLine(macro.Generate(pragma.Arguments));

                    script.Body.AddComment(System.DateTime.Now.ToString());

                    while(parentClassesCount-- > 0) script.Body.AddClosingBrace();

                    if (!string.IsNullOrEmpty(namespaces)) script.Body.AddClosingBrace();

                    context.AddSource($"{namespaces}.{cls.Identifier.Text}.{pragma.Name}.{pragma.GetHashCode()}", SourceText.From(script, Encoding.UTF8));
                }
            }
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
