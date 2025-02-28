using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class AssignExpressionAstNode : AstNode
    {
        //TODO: INFO: Should be done - 21.12.2024
        private List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "AssignExpression";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count == 1)
            {
                MeasurePointVariable res = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                thread.CurrentNode = Parent;
                return res;
            }
            else if (ChildNodes.Count == 3)
            {
                MeasurePointVariable res = new MeasurePointVariable();
                String variable = children[0].Token.ValueString;
                MeasurePointVariable value = (MeasurePointVariable)ChildNodes[2].Evaluate(thread);

                AstNode root = Parent;
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                
                if (root is ProgramAstNode)
                {
                    //hope it works
                    //search for variable in local variables
                    if ((root as ProgramAstNode).LocalVariables.ContainsKey(variable))
                    {
                        (root as ProgramAstNode).LocalVariables[variable] = value;
                    }
                    //search for variable in global variables
                    ProgramAstNode main = (root as ProgramAstNode).GetMain();
                    if (main.GlobalVariables.ContainsKey(variable))
                    {
                        main.GlobalVariables[variable] = value;
                    }
                    else
                    {
                        //if not found create new variable in local
                        (root as ProgramAstNode).LocalVariables.Add(variable, value);
                    }
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Invalid assign expression, root is not ProgramAstNode");
                }

            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Invalid assign expression, Children Count invalid");
            }
            thread.CurrentNode = Parent;
            return null;
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
