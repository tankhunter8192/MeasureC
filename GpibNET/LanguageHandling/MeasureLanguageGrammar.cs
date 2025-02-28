using System;
using System.Collections.Generic;
using System.Linq;
using Gpib.Web.LanguageHandling.CustomAstNodes;
using Gpib.Web.LanguageHandling.CustomTerminals;
using Irony.Parsing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.TextTemplating;

namespace Gpib.Web.LanguageHandling
{
    //here gets the grammar of the language defined
    [Language("MeasurementLanguage", "0.1", "A programming language for no computer scientists")]
    public class MeasureLanguageGrammar : Irony.Parsing.Grammar
    {
        public MeasureLanguageGrammar() : base(caseSensitive: false)
        {
            #region Comment

            CommentTerminal LineComment = new CommentTerminal("line-terminal", "#","\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(LineComment);
            CommentTerminal BlockComment = new CommentTerminal("block-comment", "/*", "*/");
            NonGrammarTerminals.Add(BlockComment);
            #endregion
            #region NonTerminalSymbols
            //following naming rule for the non-terminals:
            //NonTerminal <name> = new NonTerminal("<name>NT");
            NonTerminal programNonTerminal = new NonTerminal("programNT", typeof(ProgramAstNode));
            NonTerminal functionNonTerminal = new NonTerminal("functionNT", typeof(FunctionAstNode));
            NonTerminal threadNonTerminal = new NonTerminal("threadNT", typeof(ThreadAstNode));
            NonTerminal headFunctionNonTerminal = new NonTerminal("headFunctionNT", typeof(HeadFunctionAstNode));
            NonTerminal headLoopElementNonTerminal =
                new NonTerminal("headLoopElementNonTerminal", typeof(HeadLoopElementAstNode));
            NonTerminal headLoopNonTerminal = new NonTerminal("headLoopNT", typeof(HeadLoopAstNode));
            NonTerminal importsNonTerminal = new NonTerminal("importsNT", typeof(ImportsAstNode));
            NonTerminal importNonTerminal = new NonTerminal("importNT", typeof(ImportAstNode));
            NonTerminal typeNonTerminal = new NonTerminal("typeNT", typeof(TypeAstNode));
            NonTerminal timeTableNonTerminal = new NonTerminal("timeTableNT", typeof(TimeTableAstNode));
            NonTerminal timeNonTerminal = new NonTerminal("timeNT", typeof(TimeAstNode));
            NonTerminal globalsNonTerminal = new NonTerminal("globalsNT", typeof(GlobalsAstNode));
            NonTerminal singleRunNonTerminal = new NonTerminal("singleRunNT", typeof(SingleRunAstNode));
            NonTerminal loopNonTerminal = new NonTerminal("loopNT", typeof(LoopBlockAstNode));
            NonTerminal parentExpressionNonTerminal = new NonTerminal("parentExpressionNT", typeof(ParentExpressionAstNode));
            NonTerminal forHeaderNonTerminal = new NonTerminal("forHeaderNT", typeof(ForHeaderAstNode));
            NonTerminal expressionNonTerminal = new NonTerminal("expressionNT", typeof(ExpressionAstNode));
            NonTerminal blockNonTerminal = new NonTerminal("blockNT", typeof(BlockAstNode));
            NonTerminal blockContentNonTerminal = new NonTerminal("blockContentNT", typeof(BlockContentAstNode));
            NonTerminal statementsNonTerminal = new NonTerminal("statementsNT", typeof(StatementsAstNode));
            NonTerminal statementNonTerminal = new NonTerminal("statementNT", typeof(StatementAstNode));
            NonTerminal semiStatementNonTerminal = new NonTerminal("semiStatementNT", typeof(SemiStatementAstNode));
            NonTerminal simpleDeclarationsNonTerminal = new NonTerminal("simpleDeclarationsNT", typeof(SimpleDeclarationsAstNode));
            NonTerminal simpleDeclarationNonTerminal = new NonTerminal("simpleDeclarationNT", typeof(SimpleDeclarationAstNode));
            NonTerminal assignExpressionNonTerminal = new NonTerminal("assignExpressionNT", typeof(AssignExpressionAstNode));
            NonTerminal signNonTerminal = new NonTerminal("Sign", typeof(SignAstNode));
            NonTerminal identifierExpressionNonTerminal = new NonTerminal("identifierExpressionNT", typeof(IdentifierExpressionAstNode));
            NonTerminal parentArgumentsNonTerminal = new NonTerminal("parentArgumentsNT", typeof(ParentArgumentsAstNode));
            


            NonTerminal numberNonTerminal = new NonTerminal("numberNT", typeof(NumberAstNode));


            #endregion

            #region TerminalSymbols

            var number = new CustomNumberTerminal("number");
            var engineernumber = new EngineeringNumberTerminal("engineernumber");
            var identifier = new IdentifierTerminal("identifier");
            var stringLiteral = new CustomStringTerminal("string", "\"", StringOptions.AllowsAllEscapes);
            var time = new TimeTerminal("time");

            #endregion

            #region Rules

            //nonterminal.rule= "[" + terminal + "]"; // [] example
            programNonTerminal.Rule = functionNonTerminal | threadNonTerminal;

            functionNonTerminal.Rule = headFunctionNonTerminal + singleRunNonTerminal;
            threadNonTerminal.Rule = headLoopNonTerminal + singleRunNonTerminal + loopNonTerminal;

            headFunctionNonTerminal.Rule = importsNonTerminal + typeNonTerminal;
            headLoopNonTerminal.Rule = headLoopElementNonTerminal + headLoopElementNonTerminal + headLoopElementNonTerminal + headLoopElementNonTerminal;

            headLoopElementNonTerminal.Rule = importsNonTerminal | typeNonTerminal | timeTableNonTerminal | globalsNonTerminal; //unique test in node construction

            singleRunNonTerminal.Rule = ToTerm("SINGLE") + ToTerm("{") + statementsNonTerminal + ToTerm("}");
            loopNonTerminal.Rule = ToTerm("LOOP") + ToTerm("{") + statementsNonTerminal + ToTerm("}");

            importsNonTerminal.Rule = ToTerm("IMPORTS") + MakeStarRule(importsNonTerminal, ToTerm(","), importNonTerminal) + ToTerm("END IMPORTS");

            importNonTerminal.Rule = identifier;

            typeNonTerminal.Rule = ToTerm("TYPE") + identifier + ToTerm("END TYPE");

            timeTableNonTerminal.Rule = ToTerm("TIMETABLE") + timeNonTerminal + ToTerm("END TIMETABLE");

            timeNonTerminal.Rule = time; 

            globalsNonTerminal.Rule = ToTerm("GLOBALS") + MakeStarRule(globalsNonTerminal,ToTerm(";") ,assignExpressionNonTerminal) + ToTerm("END GLOBALS");

            blockNonTerminal.Rule = ToTerm("{") + blockContentNonTerminal + ToTerm("}");

            blockContentNonTerminal.Rule = simpleDeclarationsNonTerminal + statementsNonTerminal | simpleDeclarationsNonTerminal | statementsNonTerminal;

            statementsNonTerminal.Rule = MakePlusRule(statementsNonTerminal, ToTerm("\n"), statementNonTerminal);

            //TODO:Add future special statements here
            statementNonTerminal.Rule = semiStatementNonTerminal + ToTerm(";")
                                        | simpleDeclarationNonTerminal
                                        | blockNonTerminal 
                                        | ToTerm("while") + parentExpressionNonTerminal + blockNonTerminal
                                        | ToTerm("for") + forHeaderNonTerminal + blockNonTerminal
                                        | ToTerm("if") + parentExpressionNonTerminal + blockNonTerminal
                                        | ToTerm("if") + parentExpressionNonTerminal + blockNonTerminal + ToTerm("else") + blockNonTerminal
                                        | ToTerm("wait") + timeNonTerminal + ToTerm(";")
                                        | ToTerm("wait until") + timeNonTerminal + ToTerm(";")
                                        | ToTerm("print") + expressionNonTerminal + ToTerm(";")
                                        | ToTerm("return") + expressionNonTerminal + ToTerm(";")
                                        | ToTerm("break") + ToTerm(";")
                                        | ToTerm("continue") + ToTerm(";")
                                        ;

            parentExpressionNonTerminal.Rule = ToTerm("(") + expressionNonTerminal + ToTerm(")");

            forHeaderNonTerminal.Rule = ToTerm("(") + assignExpressionNonTerminal + ToTerm(";") + expressionNonTerminal + ToTerm(";") + numberNonTerminal + ToTerm(")");


            simpleDeclarationsNonTerminal.Rule = MakePlusRule(simpleDeclarationsNonTerminal, ToTerm(";"), simpleDeclarationNonTerminal);

            simpleDeclarationNonTerminal.Rule = assignExpressionNonTerminal
                                                | identifier + ToTerm(".") + identifier + parentArgumentsNonTerminal
                                                ;

            //for declaration of multiple variables names
            semiStatementNonTerminal.Rule = semiStatementNonTerminal + ToTerm(",") + identifier;

            assignExpressionNonTerminal.Rule = identifier + ToTerm("=") + expressionNonTerminal | expressionNonTerminal;

            
            expressionNonTerminal.Rule = expressionNonTerminal + signNonTerminal + expressionNonTerminal 
                                         | ToTerm("!") + expressionNonTerminal
                                         | ToTerm("(") + expressionNonTerminal + ToTerm(")")
                                         | identifierExpressionNonTerminal
                                         ;

            signNonTerminal.Rule = ToTerm("+")
                                 | ToTerm("-")
                                 | ToTerm("*")
                                 | ToTerm("/")
                                 | ToTerm("%")
                                 | ToTerm("&")
                                 | ToTerm("|")
                                 | ToTerm("^")
                                 | ToTerm("==")
                                 | ToTerm("!=")
                                 | ToTerm("<") 
                                 | ToTerm(">") 
                                 | ToTerm("<=")
                                 | ToTerm(">=")

                                 ;

            identifierExpressionNonTerminal.Rule = identifier
                                                   | numberNonTerminal
                                                   | identifier + parentArgumentsNonTerminal // function call
                                                   | identifier + ToTerm(".") + identifier + parentArgumentsNonTerminal // device function call
                                                   //| identifierExpressionNonTerminal + ToTerm(".") + identifier //class in class in ... not support in this release
                                                   | identifier + ToTerm(".") + ToTerm("valid")
                                                   | identifier + ToTerm("[") + expressionNonTerminal + ToTerm("]") //array access
                                                   ; 

            parentArgumentsNonTerminal.Rule = ToTerm("(") + MakeStarRule(parentArgumentsNonTerminal,ToTerm(";") ,assignExpressionNonTerminal) + ToTerm(")");

            numberNonTerminal.Rule = number | engineernumber | stringLiteral;

            #endregion

            #region GrammarRoot

            //Set predecence and associativity
            RegisterOperators(1, "!", "&", "|");
            RegisterOperators(2, "+", "-");
            RegisterOperators(3, "*", "/", "%");

            MarkPunctuation("[", "]", "(", ")", "{", "}");


            //example this.Root = Topvalue;
            this.Root = programNonTerminal;
            //MarkPunctuation("[", "]");
            //MarkTransient(nonterminals list);
            this.MarkReservedWords("valid","while","for","if", "else", "wait", "wait until", "print", "return", "break",
                "continue", "map", "IMPORTS", "END IMPORTS", "GLOBALS", "END GLOBALS", "TIMETABLE", "END TIMETABLE", "TYPE", "END TYPE", "SINGLE", "LOOP");
            #endregion
        }

    }
}
