using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class HeadLoopElementAstNode : AstNode
    {
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "HeadLoopElement";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node != null) AddChild("child", node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count != 1)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("HeadLoopElement must have 1 child");
            }
            else
            {
                var res = ChildNodes[0].Evaluate(thread);
                if (res is null)
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("HeadLoopElement must have a value");
                }
                else
                {
                    thread.CurrentNode = Parent;
                    return res;
                }
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
