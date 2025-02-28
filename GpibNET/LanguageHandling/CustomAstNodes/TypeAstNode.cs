using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class TypeAstNode : AstNode
    {
        //TODO: INFO: Should be done - 22.12.2024
        List<ParseTreeNode> children = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "Type";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (children.Count != 1)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Type must have one child");
            }
            else
            {
                string token = children[0].Token.ValueString;
                if (token != null)
                {
                    var root = Parent;
                    while (root.Parent != null)
                    {
                        root = root.Parent;
                    }
                    if (root is not ProgramAstNode)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Type Ast Node did NOT FOUND root/program");
                    }
                    ProgramAstNode rootProgram = (root as ProgramAstNode)!;
                    switch (token.ToLower())
                    {
                        case "thread":
                            rootProgram.IsThread = true;    
                            break;
                        case "function":
                            rootProgram.IsThread = false;
                            break;
                        
                        default:
                            ErrorAnchor = Location;
                            thread.ThrowScriptError("Type not found");
                            break;
                    }
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Type must have a value");
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
