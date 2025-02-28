using Gpib.Web.Data.DBClasses;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using Irony.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ProgramAstNode : AstNode
    {
        public Dictionary<string, MeasurePointVariable> LocalVariables { set; get; } = new Dictionary<string, MeasurePointVariable>();
        public Dictionary<string, MeasurePointVariable> GlobalVariables { set; get; } = new Dictionary<string, MeasurePointVariable>();
        public List<ParseTreeNode> ParseChildren = new List<ParseTreeNode>();
        private bool _isMain { get; set; } = true;
        private AstNode Main { get; set; } = null;
        public List<String> Imports { set; get; } = new List<string>();
        public bool IsThread { get; set; } = false;

        public override void Init(AstContext context, ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);
            AsString = "Program";
            foreach (var node in parseNode.ChildNodes)
            {
                if (node != null) AddChild("child", node);
                if (node != null) ParseChildren.Add(node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            foreach (var child in ChildNodes)
            {
                child.Evaluate(thread);
            }
            
            thread.CurrentNode = Parent;
            return null;
        }

        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            thread.CurrentNode = this;
            base.DoSetValue(thread, value);
            thread.CurrentNode = Parent;
        }

        public void SetMain(AstNode main)
        {
            _isMain = false;
            Main = main;
        }
        public ProgramAstNode GetMain()
        {
            return (this._isMain) ? this : (Main as ProgramAstNode).GetMain();
        }

        public MeasurePointVariable GetVariable(string name)
        {
            if (LocalVariables.ContainsKey(name))
            {
                return LocalVariables[name];
            }

            if (_isMain)
            {
                return GlobalVariables[name];
            }
            else
            {
                return (Main is null) ? (Main as ProgramAstNode).GetVariable(name) : new MeasurePointVariable(MPVPrimeType.Error);
            }
            return null;
        }

        public override string ToString()
        {
            string mes = "";
            mes += "programNode\n[\n";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            mes += "]\n";
            return mes;
        }
    }
}
