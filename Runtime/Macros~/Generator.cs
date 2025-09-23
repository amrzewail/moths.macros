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
            try
            {
                Compilation compilation = context.Compilation;

                List<(SyntaxTree tree, ClassDeclarationSyntax cls)> classes = new();
                Dictionary<SyntaxTree, List<DirectiveTriviaSyntax>> directives = new();

                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    var root = syntaxTree.GetRoot();
                    var classesList = root.DescendantNodes()
                                      .OfType<ClassDeclarationSyntax>();
                    foreach (var classDeclaration in classesList) classes.Add((syntaxTree, classDeclaration));

                    if (!directives.ContainsKey(syntaxTree)) directives[syntaxTree] = new List<DirectiveTriviaSyntax>();
                    var dirs = directives[syntaxTree];

                    directives[syntaxTree].AddRange(root.DescendantTrivia()
                        .Where(t => t.IsDirective)                // only directives
                        .Select(t => t.GetStructure())            // get structured node
                        .OfType<DirectiveTriviaSyntax>()          // cast
                        .Where(d => d.ToFullString().Contains("#pragma Macro")));
                }

                Dictionary<string, (Macro, SyntaxList<UsingDirectiveSyntax>)> macros = new Dictionary<string, (Macro, SyntaxList<UsingDirectiveSyntax>)>();

                foreach (var c in classes)
                {
                    var cls = c.cls;
                    var tree = c.tree;

                    if (!HasAttribute(cls, "Macro")) continue;

                    Macro macro = new Macro(cls);
                    macros.Add(cls.Identifier.Text, (macro, GetUsings(cls)));

                    for (int i = 0; i < macro.Args.Count; i++)
                    {
                        var arg = macro.Args[i];
                        if (arg.Trim().StartsWith("type:"))
                        {
                            var typeName = arg.Trim().Substring("type:".Length);
                            var namespaces = GetFullNamespace(GetOutsideNamespace(cls, tree.GetRoot()), context);
                            context.AddSource($"{namespaces}{cls.Identifier.Text}{typeName}", SourceText.From(new ArgStructTemplate(namespaces, cls.Identifier.Text, typeName), Encoding.UTF8));
                        }
                    }
                }

                List<(Pragma pragma, TypeDeclarationSyntax cls, NamespaceDeclarationSyntax nms, SyntaxNode node)> pragmas = new();

                foreach (var tree in directives)
                {
                    var syntaxNode = tree.Key.GetRoot();

                    foreach (var dir in tree.Value)
                    {
                        var directive = dir;
                        var directiveText = directive.ToFullString();

                        pragmas.Add((new Pragma(directiveText), GetOutsideType(dir, syntaxNode), GetOutsideNamespace(dir, syntaxNode), directive));
                    }
                }

                Protection protection = Protection.None;
                Binding binding = Binding.Member;
                Mutability mutability = Mutability.Mutable;

                foreach (var p in pragmas)
                {
                    var pragma = p.pragma;
                    var cls = p.cls;
                    var nms = p.nms;
                    var node = p.node;

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

                    script.AddLine($"//{System.DateTime.Now.ToString()}");

                    foreach (var us in usings)
                    {
                        script.AddLine(us.ToFullString());
                    }

                    foreach (var us in macroUsings)
                    {
                        script.AddLine(us.ToFullString());
                    }

                    string namespaces = nms != null ? GetFullNamespace(nms, context) : "";

                    if (!string.IsNullOrEmpty(namespaces)) script.AddLine($"namespace {namespaces}\n{{");

                    int parentClassesCount = 0;
                    string classHierarchy = "";
                    string parentClasses = cls != null ? GetParentClasses(cls, out parentClassesCount, out classHierarchy) : "";

                    if (!string.IsNullOrEmpty(parentClasses)) script.AddLine(parentClasses);

                    if (cls != null)
                    {
                        //script.AddClass(protection, binding, Modifier.Partial, mutability, cls.Identifier.Text);
                        script.AddLine($"partial {GetTypeDeclarationName(cls)} {cls.Identifier.Text}");
                        script.Body.AddLine(macro.Generate(pragma.Arguments));
                        while (parentClassesCount-- > 0) script.Body.AddClosingBrace();
                        if (!string.IsNullOrEmpty(namespaces)) script.Body.AddClosingBrace();
                    }
                    else
                    {
                        script.AddLine(macro.Generate(pragma.Arguments));
                        script.ClearBody();
                        if (!string.IsNullOrEmpty(namespaces)) script.AddLine("}\n");
                    }

                    string sourceName = $"{(!string.IsNullOrEmpty(namespaces) ? namespaces + "." : "")}{classHierarchy}{(cls != null ? cls.Identifier.Text + "." : "")}";

                    context.AddSource($"{sourceName}{pragma.Name}.{pragma.GetHashCode()}", SourceText.From(script, Encoding.UTF8));
                }

                context.AddSource("Success", SourceText.From($"//{System.DateTime.Now}", Encoding.UTF8));
            }
            catch (System.Exception e)
            {
                context.AddSource("Error", SourceText.From($"/*{System.DateTime.Now}\n{e}", Encoding.UTF8));
            }
        }

        private string GetTypeDeclarationName(TypeDeclarationSyntax type)
        {
            if (type is ClassDeclarationSyntax) return "class";
            else if (type is InterfaceDeclarationSyntax) return "interface";
            else if (type is StructDeclarationSyntax) return "struct";
            else if (type is RecordDeclarationSyntax) return "record";
            return "";
        }

        private TypeDeclarationSyntax GetOutsideType(SyntaxNode directive, SyntaxNode root)
        {
            var position = directive.SpanStart;
            return root.DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .LastOrDefault(c => c.Span.Contains(position));
        }

        private NamespaceDeclarationSyntax GetOutsideNamespace (SyntaxNode directive, SyntaxNode root)
        {
            var position = directive.SpanStart;
            return root.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .LastOrDefault(c => c.Span.Contains(position));
        }

        private string GetParentClasses(TypeDeclarationSyntax classNode, out int count, out string classHierarchy)
        {
            var classes = new List<TypeDeclarationSyntax>();
            var current = classNode.Parent as TypeDeclarationSyntax;
            while (current != null)
            {
                classes.Insert(0, current); // Insert at start for outermost first
                current = current.Parent as TypeDeclarationSyntax;
            }

            string result = "";
            classHierarchy = "";
            count = 0;
            foreach (var cls in classes)
            {
                string modifiers = cls.Modifiers.ToFullString().Trim();
                string partial = modifiers.Contains("partial") ? "partial" : "";
                result += new string(' ', count * 4) + $"{partial} {GetTypeDeclarationName(cls)} {cls.Identifier.Text} {{\n";
                classHierarchy += $"{cls.Identifier.Text}.";
                count++;
            }

            return result;
        }

        private string GetFullNamespace(NamespaceDeclarationSyntax nmsNode, GeneratorExecutionContext context)
        {
            string namespaces = string.Empty;
            if (nmsNode == null) return namespaces;
            var model = context.Compilation.GetSemanticModel(nmsNode.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(nmsNode);
            var nms = symbol?.ContainingNamespace?.ToDisplayString();
            if (nms != "<global namespace>") namespaces += nms + (nmsNode != null ? "." : "");
            if (nmsNode != null) namespaces += nmsNode.Name.ToString().Split('.').Last();
            return namespaces;
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
