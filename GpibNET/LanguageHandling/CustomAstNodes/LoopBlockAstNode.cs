using Irony.Ast;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Irony.Interpreter.Ast;
using NuGet.Protocol;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class LoopBlockAstNode : AstNode
    {
        public TimeSpan LoopInterval { get; set; }
        public TimeOnly SpecificTime { get; set; }
        public List<AstNode> ChildNodes { get; set; } = new List<AstNode>();

        public override void Init(AstContext context, ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);
            foreach (var child in Parent.ChildNodes)
            {
                try
                {
                    LoopInterval = TimeSpan.Parse("");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            foreach (var child in parseNode.ChildNodes[1].ChildNodes)
            {
                if (child != null)
                {
                    ChildNodes.Add(AddChild("child",child));
                }
            }
        }
        /*
        public async Task<object> EvaluateNodeAsync(Irony.Interpreter.ScriptThread thread)
        {
            while (true)
            {
                foreach (var child in ChildNodes)
                {
                    if (child is LoopBlockAstNode loopChild)
                    {
                        await loopChild.EvaluateNodeAsync(thread);
                    }
                    else
                    {
                        //TODO: Evaluate child that is not a loop
                    }
                }
                await Task.Delay(LoopInterval);
            }

            
        }*/

        public object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            //TODO: Implement loop logic
            foreach (var child in ChildNodes)
            {
                child.Evaluate(thread);
            }
            thread.CurrentNode = Parent;
            return null;
        }
        public void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            thread.CurrentNode = this;
            base.SetValue(thread, value);
            thread.CurrentNode = Parent;
        }

        public override string ToString()
        {
            string mes = "";
            mes += $"LoopBlockAstNode: {LoopInterval} {SpecificTime}\n";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            return mes;
        }
    }
}
