using System.Runtime.InteropServices.Marshalling;
using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class StatementAstNode : AstNode
    {
        private AstNode Assinement { get; set; }
        private AstNode Condition { get; set; }
        private AstNode Increment { get; set; }
        public override void Init(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);
            AsString = "Statement";
            foreach (var node in parseNode.ChildNodes)
            {
                if(node != null) AddChild("child", node);
            }
        }
        protected override object DoEvaluate (Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (!(ChildNodes.Count >= 3))
            {
                ErrorAnchor = Span.Location;
                thread.CurrentNode = Parent;
                return null!;
            }
            switch (ChildNodes[0].Term.ToString().ToLower())
            {
                case "while":
                    bool condition = true;
                    while (condition)
                    {
                        condition = (bool)ChildNodes[1].Evaluate(thread);
                    }
                    break;
                case "if":
                    bool conditionIf = (bool)ChildNodes[1].Evaluate(thread);
                    if ((!ChildNodes[3].Term.ToString().ToLower().Equals("else"))&&(ChildNodes.Count < 4))
                    {
                        if (conditionIf)
                        {
                            ChildNodes[2].Evaluate(thread);
                        }
                    }
                    else
                    {
                        //if extended
                        if (conditionIf)
                        {
                            ChildNodes[2].Evaluate(thread);
                        }
                        else
                        {
                            ChildNodes[4].Evaluate(thread);
                        }
                    }
                    break;
                case "for":
                    bool conditionfor = true;
                    ChildNodes[1].ChildNodes[1].Evaluate(thread);
                    conditionfor = (bool)ChildNodes[1].ChildNodes[3].Evaluate(thread);
                    while (conditionfor)
                    {
                        ChildNodes[2].Evaluate(thread);
                        ChildNodes[1].ChildNodes[5].Evaluate(thread);
                    }
                    break;
                default:
                    ErrorAnchor = Span.Location;
                    thread.CurrentNode = Parent;
                    return null!;
            }
            thread.CurrentNode = Parent;
            return null;
        }

        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            base.DoSetValue(thread, value);
        }
        public override string ToString()
        {
            string mes = "";
            mes += "statementNode [\n";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            mes += "\n]\n";
            return mes;
        }
    }
}
