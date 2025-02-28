using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ImportAstNode : AstNode
    {
        List<ParseTreeNode> children = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "Import";
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
                thread.ThrowScriptError("Import error: children count wrong");
            }
            else
            {
                string token = children[0].Token.ValueString;
                var root = Parent;
                while (root.Parent != null)
                {
                    root = root.Parent;
                }
                if(token != null && token.Length > 0)
                {
                    if (root is not ProgramAstNode)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Import error: root is not ProgramAstNode");
                    }
                    var function = (root as ProgramAstNode)!;
                    if(!function.Imports.Contains(token))
                    {
                        function.Imports.Add(token);
                    }
                    function.Imports.Add(token);
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Import error: token is null or empty");
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
