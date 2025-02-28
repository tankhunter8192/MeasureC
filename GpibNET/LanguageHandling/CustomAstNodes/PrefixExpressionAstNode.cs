using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    //TODO: Should be done - 21.12.2024
    public class PrefixExpressionAstNode : AstNode
    {
        public List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public ParseTreeNode ThisNode { get; set; }
        public override void Init(AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            ThisNode = treeNode;
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
            AsString = "PrefixExpression";
        }


        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            
            if (ChildNodes.Count == 1)
            {
                var res = ChildNodes[0].Evaluate(thread);
                thread.CurrentNode = Parent;
                return res;
            } 
            else if (ChildNodes.Count == 2)
            {
                var res = ChildNodes[1].Evaluate(thread);
                if (res is MeasurePointVariable)
                {
                    res = (res != null) ? (!(res as MeasurePointVariable)) : (new MeasurePointVariable(MPVPrimeType.Error));
                    thread.CurrentNode = Parent;
                    return res;
                }
            }
            ErrorAnchor = Location;
            thread.ThrowScriptError("Invalid prefix expression");
            thread.CurrentNode = Parent;
            return null;
        }
        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
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
