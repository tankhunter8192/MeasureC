using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class MultiplyOperatorAstNode : AstNode
    {
        //TODO: should be done - 21.12.2024
        public List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public ParseTreeNode ThisNode { get; set; }
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "MultiplyOperator";
            ThisNode = treeNode;
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
                MeasurePointVariable res = new MeasurePointVariable(children[0].Token.ValueString);
                thread.CurrentNode = Parent;
                return res;
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
