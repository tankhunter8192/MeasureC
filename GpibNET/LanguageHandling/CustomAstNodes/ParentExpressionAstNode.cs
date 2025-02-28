using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ParentExpressionAstNode : AstNode
    {
        //TODO: INFO: Should be done - 22.12.2024
        //Is just Brackets -> no calculation needed, just passthrough the value
        List<ParseTreeNode> children = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "ParentExpression";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count != 3)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Invalid parent expression: wrong Children count");
            }
            else
            {
                MeasurePointVariable? res = new MeasurePointVariable();
                var varres = ChildNodes[1].Evaluate(thread);
                if (varres is MeasurePointVariable)
                {
                    res = varres as MeasurePointVariable;
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Invalid parent expression: wrong type");
                }
                thread.CurrentNode = Parent;
                return res!;
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
