using Gpib.Web.Data;
using Gpib.Web.Data.DBClasses;
using Gpib.Web.LanguageHandling.CustomAstNodes;
using Irony;
using Irony.Interpreter;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling
{
    public class MeasureLanguage
    {
        private LanguageData _language { get; set; }
        private LanguageRuntime _runtime { get; set; }
        private ScriptApp _app { get; set; }
        private ScriptThread _scriptThread { get; set; }
        private Dictionary<string, MeasurePointVariable> _globalVariables { get; set; }
        private Dictionary<string, MeasurePointVariable> _localVariables { get; set; }
        private Parser _parser { get; set; }
        private MeasureLanguageGrammar _grammar { get; set; }

        public MeasureLanguage()
        {
            _grammar = new MeasureLanguageGrammar();
            _parser = new Parser(_grammar);
            _globalVariables = new Dictionary<string, MeasurePointVariable>();
            _localVariables = new Dictionary<string, MeasurePointVariable>();
            _runtime = new LanguageRuntime(_language);
            _app = new ScriptApp(_runtime);
            _scriptThread = new ScriptThread(_app);
        }

        public void AddGlobals(Dictionary<string, MeasurePointVariable> global)
        {
            foreach (var kvp in global)
            {
                if (_globalVariables.TryGetValue(kvp.Key, out var value))
                {
                    _globalVariables[kvp.Key] = kvp.Value;
                }
                else
                {
                    _globalVariables.Add(kvp.Key, kvp.Value);
                }
            }
        }
        public Dictionary<string,MeasurePointVariable> GetGlobals()
        {
            return _globalVariables;
        }
        public void AddLocals(Dictionary<string, MeasurePointVariable> local)
        {
            foreach (var kvp in local)
            {
                if (_localVariables.TryGetValue(kvp.Key, out var value))
                {
                    _localVariables[kvp.Key] = kvp.Value;
                }
                else
                {
                    _localVariables.Add(kvp.Key, kvp.Value);
                }
            }
        }
        public MeasurePointVariable Execute(ProgramFile file)
        {
            var parseTree = _parser.Parse(file.Content);
            if (parseTree.HasErrors())
            {
                foreach (var error in parseTree.ParserMessages)
                {
                    GlobalStaticVariables.Logger.LogError(file.Name + " Execution",error.Message);
                }
            }

            _scriptThread.CurrentScope.SetValue(0, _globalVariables);
            _scriptThread.CurrentScope.SetValue(1, _localVariables);
            var root = parseTree.Root.AstNode as ProgramAstNode;

            MeasurePointVariable result = (MeasurePointVariable) root!.Evaluate(_scriptThread);
            return result;
        }

        public List<Irony.LogMessage> Test(ProgramFile file)
        {
            var parseTree = _parser.Parse(file.Content);
            if (parseTree.HasErrors())
            {
                return new List<Irony.LogMessage>(parseTree.ParserMessages);
            }

            return new List<Irony.LogMessage>();
        }
    }
}
