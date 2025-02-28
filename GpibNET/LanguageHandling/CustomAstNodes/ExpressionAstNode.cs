using System.Reflection.Metadata;
using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ExpressionAstNode : AstNode
    {
        private List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "Expression";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count == 1)
            {
                MeasurePointVariable res = new MeasurePointVariable();
                res = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                thread.CurrentNode = Parent;
                return res;
            }
            else if (ChildNodes.Count == 2)
            {
                if (children[0].Token.Text.Equals("!"))
                {
                    MeasurePointVariable res = !((MeasurePointVariable)ChildNodes[1].Evaluate(thread));
                    thread.CurrentNode = Parent;
                    return res;
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Error in expression: 2 Children, but no ! Operator not found");
                }
            }
            else if (ChildNodes.Count == 3)
            {
                if (children[0].Token.Text.Equals("(") && children[2].Token.Text.Equals(")"))
                {
                    MeasurePointVariable tmp = (MeasurePointVariable)ChildNodes[1].Evaluate(thread);
                    thread.CurrentNode = Parent;
                    return tmp;
                }

                if (ChildNodes[1] is not SignAstNode)
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Error in expression: 3 Children, but no Sign Operator not found");
                }
                string sign = children[1].Token.Text;
                MeasurePointVariable var1 = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                MeasurePointVariable var2 = (MeasurePointVariable)ChildNodes[2].Evaluate(thread);
                MeasurePointVariable res;
                switch (sign)
                {
                    case "+":
                        res = var1 + var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "-":
                        res = var1 - var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "*":
                        res = var1 * var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "/":
                        res = var1 / var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "%":
                        res = var1 % var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "|":
                        res = var1 | var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "&":
                        res = var1 & var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "^":
                        res = var1 ^ var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "==":
                        res = var1 == var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "!=":
                        res = var1 != var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case ">":
                        res = var1 > var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "<":
                        res = var1 < var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case "<=":
                        res = var1 <= var2;
                        thread.CurrentNode = Parent;
                        return res;
                    case ">=":
                        res = var1 >= var2;
                        thread.CurrentNode = Parent;
                        return res;
                }
            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Error in expression: Children count");
            }
            thread.CurrentNode = Parent;
            return null!;
        }

        public override void DoSetValue(ScriptThread thread, object value)
        {
            base.DoSetValue(thread, value);
        }

        public override string ToString()
        {
            string mes = AsString;
            mes += "\n[";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            mes += "]\n";
            return mes;
        }
    }
}
