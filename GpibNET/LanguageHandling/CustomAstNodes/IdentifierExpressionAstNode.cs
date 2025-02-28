using Gpib.Web.Data;
using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Protocol;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class IdentifierExpressionAstNode : AstNode
    {
        List<ParseTreeNode> children = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "IdentifierExpression";
            foreach (var node in treeNode.ChildNodes)
            {
                if(node is not null) AddChild("child", node);
                if(node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count == 1)
            {
                if (ChildNodes[0] is NumberAstNode)
                {
                    MeasurePointVariable tmp = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                    thread.CurrentNode = Parent;
                    return tmp; 
                }

                if (ChildNodes[0] is IdentifierNode)
                {
                    string name = children[0].Token.ValueString;
                    var _glo = thread.CurrentScope.GetValue(0);
                    var _loc = thread.CurrentScope.GetValue(1);
                    if (_glo is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Global variables not found");
                    }
                    if (_loc is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Local variables not found");
                    }
                    if(((Dictionary<string,MeasurePointVariable>)_loc!).ContainsKey(name)){
                        MeasurePointVariable res = ((Dictionary<string, MeasurePointVariable>)_loc!)[name];
                        thread.CurrentNode = Parent;
                        return res;
                    }
                    else
                    {
                        if (((Dictionary<string, MeasurePointVariable>)_glo!).ContainsKey(name))
                        {
                            MeasurePointVariable res = ((Dictionary<string, MeasurePointVariable>)_glo!)[name];
                        }
                        ((Dictionary<string, MeasurePointVariable>)_loc!).Add(name, new MeasurePointVariable(Decimal.Zero));
                    }
                }
            }
            else if (ChildNodes.Count == 2)
            {
                //function call
                if ((ChildNodes[0].Term is IdentifierTerminal) && (ChildNodes[1].Term is ParentArgumentsAstNode))
                {
                    //TODO: Implement this (function call)
                    var name = children[0].Token.ValueString;
                    var function = GlobalStaticVariables.DbContext.ProgramFiles.FirstOrDefault(f => f.Name == name);
                    if (function is null)
                    {
                        if (name.ToLower().Equals("map"))
                        {
                            var maps = ChildNodes[1].Evaluate(thread);
                            if (maps is List<MeasurePointVariable>)
                            {
                                if (((List<MeasurePointVariable>)maps).Count == 3)
                                {
                                    MeasurePointVariable res = MeasurePointVariable.Map(((List<MeasurePointVariable>)maps)[0], ((List<MeasurePointVariable>)maps)[1], ((List<MeasurePointVariable>)maps)[2]);
                                } else if (((List<MeasurePointVariable>)maps).Count == 2)
                                {
                                    MeasurePointVariable res = MeasurePointVariable.Map(((List<MeasurePointVariable>)maps)[0], ((List<MeasurePointVariable>)maps)[1]);
                                }
                                else
                                {
                                    ErrorAnchor = Location;
                                    thread.ThrowScriptError("Map function needs 2 or 3 arguments");
                                }
                            }
                        }
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Function not found");
                    }
                    var list = ChildNodes[1].Evaluate(thread);

                    //function call
                    MeasureLanguage lm = new MeasureLanguage();
                    lm.AddGlobals((Dictionary<string,MeasurePointVariable>)thread.CurrentScope.GetValue(0));
                    //list->dictionary
                    Dictionary<string, MeasurePointVariable> locals = new Dictionary<string, MeasurePointVariable>();
                    if (list is List<MeasurePointVariable>)
                    {
                        foreach (var item in (List<MeasurePointVariable>)list)
                        {
                            locals.Add(item.Name, item);
                        }
                    }
                    lm.AddLocals(locals);
                    var res1 = lm.Execute(function!);
                    //sync globals
                    var retg = lm.GetGlobals();
                    foreach (var item in retg)
                    {
                        ((Dictionary<string, MeasurePointVariable>)thread.CurrentScope.GetValue(0)!)[item.Key] = item.Value;
                    }
                    thread.CurrentNode = Parent;
                    return res1;
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Function call did not get identifier or parent expression");
                }
            }
            else if (ChildNodes.Count == 3)
            {
                //validity check
                if (ChildNodes[0] is IdentifierNode && children[2].Token.Text.ToLower().Equals("valid"))
                {
                    //find var
                    string name = children[0].Token.ValueString;
                    var _glo = thread.CurrentScope.GetValue(0);
                    var _loc = thread.CurrentScope.GetValue(1);
                    if (_glo is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Global variables not found");
                    }
                    if (_loc is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Local variables not found");
                    }
                    if (((Dictionary<string, MeasurePointVariable>)_loc!).ContainsKey(name))
                    {
                        MeasurePointVariable res = ((Dictionary<string, MeasurePointVariable>)_loc!)[name];
                        thread.CurrentNode = Parent;
                        return res.IsValid ? new MeasurePointVariable(Decimal.One) : new MeasurePointVariable(Decimal.Zero);
                    }
                    else
                    {
                        if (((Dictionary<string, MeasurePointVariable>)_glo!).ContainsKey(name))
                        {
                            MeasurePointVariable res = ((Dictionary<string, MeasurePointVariable>)_glo!)[name];
                            thread.CurrentNode = Parent;
                            return res.IsValid ? new MeasurePointVariable(Decimal.One): new MeasurePointVariable(Decimal.Zero);
                        }
                        else
                        {
                            thread.CurrentNode = Parent;
                            return new MeasurePointVariable(Decimal.Zero);
                        }
                    }
                }
            }
            else if (ChildNodes.Count == 4)
            {
                //is device function call
                //TODO: Implement function call, check Arguments call
                if ((ChildNodes[0] is IdentifierNode) && (ChildNodes[2] is IdentifierNode) && (ChildNodes[3] is ParentArgumentsAstNode))
                {
                    
                    var item = children[0].Token.ValueString; //device name
                    var function = children[2].Token.ValueString; //function name
                    var List = ChildNodes[3].Evaluate(thread);
                    var device = GlobalStaticVariables.DbContext.Devices.FirstOrDefault(f => f.Name == item);
                    if (device is null)
                    {
                        device = GlobalStaticVariables.DbContext.Devices.FirstOrDefault(f => f.SerialNumber == item);
                        //why not, but it is uncommon to know the serial number of the device out of the head
                        if (device is null)
                        {
                            device = GlobalStaticVariables.DbContext.Devices.FirstOrDefault(f => f.ConnectionString == item);
                            //unexpected to work, if one writes the connection string in the script then it is a better idea to use pyvisa directly in jupyter notebook 
                        }
                    }

                    if (device is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Device not found");
                    }
                    var functionsInfo = device!.FunctionDevices;
                    if (functionsInfo is null)
                    {
                        functionsInfo = GlobalStaticVariables.DbContext.ProfileDevices.FirstOrDefault(p => p.GPIBDevices.Contains(device)) as ICollection<FunctionDevice>;
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Device has no functions collection");
                    }
                    //TODO: finish List of params
                    var functionInfo = functionsInfo!.FirstOrDefault(f => f.Name == function);
                    if (functionInfo is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Function not found");
                    }
                    //TODO: rework this
                    var res = GlobalStaticVariables.PyVisaWrapper.Measure(device, functionInfo!, List);
                    thread.CurrentNode = Parent;
                    return res;
                    
                }
                //Point Access
                if ((ChildNodes[0] is IdentifierNode) && (ChildNodes[2] is ExpressionAstNode))
                {
                    //Point Access
                    string name = children[0].Token.ValueString;
                    var _glo = thread.CurrentScope.GetValue(0);
                    var _loc = thread.CurrentScope.GetValue(1);
                    if (_glo is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Global variables not found");
                    }
                    if (_loc is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Local variables not found");
                    }

                    MeasurePointVariable tar;
                    MeasurePointVariable index = (MeasurePointVariable)ChildNodes[2].Evaluate(thread);
                    if (((Dictionary<string, MeasurePointVariable>)_loc!).ContainsKey(name))
                    {
                        tar = ((Dictionary<string, MeasurePointVariable>)_loc!)[name];
                    }
                    else
                    {
                        if (((Dictionary<string, MeasurePointVariable>)_glo!).ContainsKey(name))
                        {
                            tar = ((Dictionary<string, MeasurePointVariable>)_glo!)[name];
                        }
                        else
                        {
                            thread.CurrentNode = Parent;
                            return new MeasurePointVariable(Decimal.Zero);
                        }
                    }
                    if (tar is null)
                    {
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("Target not found");
                    }

                }
            }
            else
            {
                //not defined
                ErrorAnchor = Location;
                thread.ThrowScriptError("Identifier not defined");

            }
            thread.CurrentNode = Parent;
            return null;
        }

        private string FindIdentifierName(ParseTreeNode node)
        {
            if (node == null) return null;
            if (node.Term is IdentifierTerminal)
            {
                return node.Token.ValueString;
            }

            foreach (var child in children)
            {
                if(child is null) return null;
                string result = FindIdentifierName(child);
                if (result != null) return result;
            }
            return null;
        }

        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            thread.CurrentNode = this;
            base.DoSetValue(thread, value);
            thread.CurrentNode = Parent;
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
