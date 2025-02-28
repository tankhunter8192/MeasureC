using Gpib.Web.Data.DBClasses;
using Gpib.Web.LanguageHandling.CustomTerminals;
using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class NumberAstNode : AstNode
    {
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "Number";
            foreach (var node in treeNode.ChildNodes)
            {
                if(node is not null) AddChild("child", node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            var child = ChildNodes.FirstOrDefault();
            if (child is null)
            {
                thread.CurrentNode = Parent;
                return null!;

            }
            MeasurePointVariable value = new MeasurePointVariable();
            if (child.Term is EngineeringNumberTerminal)
            {
               
                EngineeringNumberTerminal? Engineer = child.Term as EngineeringNumberTerminal;
                value = Engineer.value;
                if (value is not null)
                {
                    thread.CurrentNode = Parent;
                    return value;
                }
            }else if (child.Term is CustomNumberTerminal)
            {
                CustomNumberTerminal? Number = child.Term as CustomNumberTerminal;
                value = Number.value;
                if (value is not null)
                {
                    thread.CurrentNode = Parent;
                    return value;
                }
            }else if (child.Term is CustomStringTerminal)
            {
                CustomStringTerminal? String = child.Term as CustomStringTerminal;
                value = String.value;
                if (value is not null)
                {
                    thread.CurrentNode = Parent;
                    return value;
                }
            }else if (child.Term is TimeTerminal)
            {
                TimeTerminal? Time = child.Term as TimeTerminal;
                value = Time.value;
                if (value is not null)
                {
                    thread.CurrentNode = Parent;
                    return value;
                }
            }
            else
            {
                ErrorAnchor= child.Span.Location;
                thread.CurrentNode = Parent;
                return null!;
            }
            thread.CurrentNode = Parent;
            return null!;
        }

        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            base.DoSetValue(thread, value);
        }

        public override string ToString()
        {
            string mes = AsString + "\n[";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            mes += "]\n";
            return mes;
        }

    }
}
