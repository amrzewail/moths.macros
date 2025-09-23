using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Moths.Macros
{
    public class MacroExpressionRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // Case 2 & 3: Macro.Expression("...").Call(...)
            if (node.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == "Call" &&
                memberAccess.Expression is InvocationExpressionSyntax innerInvocation &&
                IsMacroExpression(innerInvocation, out var methodName))
            {
                var newExpr = SyntaxFactory.ParseExpression(methodName);

                // With arguments → methodName(args...)
                if (node.ArgumentList.Arguments.Count > 0)
                {
                    return SyntaxFactory.InvocationExpression(newExpr, node.ArgumentList);
                }

                // No arguments → methodName()
                return SyntaxFactory.InvocationExpression(
                    newExpr,
                    SyntaxFactory.ArgumentList()
                );
            }

            // Case 4: Inline usage → var x = Macro.Expression("...")
            if (IsMacroExpression(node, out var exprBody))
            {
                return SyntaxFactory.ParseExpression(exprBody);
            }

            return base.VisitInvocationExpression(node);
        }

        private bool IsMacroExpression(InvocationExpressionSyntax node, out string body)
        {
            body = null;
            if (node.Expression.ToString().StartsWith("Macro.Expression") &&
                node.ArgumentList.Arguments.FirstOrDefault()?.Expression is LiteralExpressionSyntax literal)
            {
                body = literal.Token.ValueText;
                return true;
            }
            return false;
        }
    }
}