using System;
namespace SharpLox
{
    public class AstPrinter : Expr.Visitor<String>
    {
        public AstPrinter()
        {
        }

        public string Print(Expr expression) {
            return expression.Accept(this);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            string str = $"({name}";

            foreach (Expr expr in exprs)
            {
                str += " ";
                str += expr.Accept(this);
            }

            str += ")";

            return str;
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.op.lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.op.lexeme, expr.right);
        }
    }
}
