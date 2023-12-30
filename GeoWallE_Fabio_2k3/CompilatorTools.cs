using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public class CompilatorTools
    {
        public static Dictionary<string, TokenType> KeywordsPool = new Dictionary<string, TokenType>()
        {
            {"and" , TokenType.AndKwToken},
            {"arc" , TokenType.ArcKwToken},
            {"black" , TokenType.ColorValueToken},
            {"blue" , TokenType.ColorValueToken},
            {"circle" , TokenType.CircleKwToken},
            {"color" , TokenType.ColorKwToken},
            {"count" , TokenType.SystemFunctionKwToken},
            {"cyan" , TokenType.ColorValueToken},
            {"draw" , TokenType.DrawKwToken},
            {"else" , TokenType.ElseKwToken},
            {"gray" , TokenType.ColorValueToken},
            {"green" , TokenType.ColorValueToken},
            {"if" , TokenType.IfKwToken},
            {"in" , TokenType.InKwToken},
            {"intersect" , TokenType.SystemFunctionKwToken},
            {"import" , TokenType.ImportKwToken},
            {"let" , TokenType.LetKwToken},
            {"line" , TokenType.LineKwToken} ,
            {"magenta" , TokenType.ColorValueToken},
            {"measure" , TokenType.SystemFunctionKwToken},
            {"not" , TokenType.NotKwToken},
            {"or" , TokenType.OrKwToken},
            {"point" , TokenType.PointKwToken},
            {"points" , TokenType.SystemFunctionKwToken},
            {"print" , TokenType.SystemFunctionKwToken} ,
            {"randoms" , TokenType.SystemFunctionKwToken},
            {"ray" , TokenType.RayKwToken},
            {"red" , TokenType.ColorValueToken},
            {"restore" , TokenType.RestoreKwToken},
            {"samples" , TokenType.SystemFunctionKwToken},
            {"segment" , TokenType.SegmentKwToken},
            {"sequence" , TokenType.SequenceKwToken},
            {"then" , TokenType.ThenKwToken},
            {"undefined" , TokenType.UndefinedKwToken},
            {"white" , TokenType.ColorValueToken},
            {"yellow" , TokenType.ColorValueToken},
        };

        public static Dictionary<string, TokenType> OperatorsPool = new Dictionary<string, TokenType>()
        {
            {"+"  , TokenType.PlusSignToken},
            {"-"  , TokenType.MinusSignToken},
            {"*"  , TokenType.StarToken},
            {"/"  , TokenType.SlashToken},
            {"^"  , TokenType.ExponentToken} ,
            {"%"  , TokenType.PercentageSignToken},
            {"("  , TokenType.OpenParenthesisToken},
            {")"  , TokenType.CloseParenthesisToken},
            {"{"  , TokenType.OpenKeyToken},
            {"}"  , TokenType.CloseKeyToken},
            {","  , TokenType.CommaSeparatorToken},
            {";"  , TokenType.SemiColonToken},
            {"!=" , TokenType.NotEqualsToken},
            {"="  , TokenType.EqualsToken},
            {"==" , TokenType.EqualsEqualsToken},
            {">"  , TokenType.GreaterToken},
            {">=" , TokenType.GreaterEqualsToken},
            {"<"  , TokenType.LessToken},
            {"<=" , TokenType.LessEqualsToken},
            {"...", TokenType.TripleDotToken},
            {"\0" , TokenType.EndOfFileToken},
        };

        // Lexer
        public static TokenType GetKeyWordKind(string word)
        {
            if (KeywordsPool.ContainsKey(word))
                return KeywordsPool[word];

            else
                return TokenType.IdentifierToken;
        }

        // Funciones
        public static List<Executable_Function> FunctionPool = new List<Executable_Function>();

        internal static Executable_Function SearchFunction(string name, int count)
        {
            foreach (var item in FunctionPool)
            {
                if (item.Match(name, count))
                    return item;
            }

            throw new InvalidOperationException($"!Syntactic Error : Function \"{name}\" receiving {count} arguments does not exist");
        }

        internal static void AddFunction(Executable_Function function)
        {
            FunctionPool.Add(function);
        }

        internal static void LoadSystemFunctions()
        {
            AddFunction(new PredefinedFunction("count", 1, WalleType.Number, System_Functions_Pool.Count));
            AddFunction(new PredefinedFunction("print", 1, WalleType.Text, System_Functions_Pool.Print));
            AddFunction(new PredefinedFunction("measure", 2, WalleType.Measure, System_Functions_Pool.Medidas));
            AddFunction(new PredefinedFunction("line", 2, WalleType.Line, System_Functions_Pool.Line));
            AddFunction(new PredefinedFunction("segment", 2, WalleType.Line, System_Functions_Pool.Segment));
            AddFunction(new PredefinedFunction("ray", 2, WalleType.Line, System_Functions_Pool.Ray));
            AddFunction(new PredefinedFunction("circle", 2, WalleType.Circle, System_Functions_Pool.Circle));
            AddFunction(new PredefinedFunction("arc", 4, WalleType.Arc, System_Functions_Pool.Arc));
            AddFunction(new PredefinedFunction("intersect", 2, WalleType.Sequence, System_Functions_Pool.Intersect));

        }


        // Colores
        public static Stack<string> ColorPool = new Stack<string>();

        internal static void AddColor(string color)
        {
            ColorPool.Push(color);
        }
        internal static void RestoreColor()
        {
            if (ColorPool.Count > 1)
                ColorPool.Pop();
        }

        // comprobar si una expresión dada es de tipo figura
        internal static bool IsFigure(IExpression expr)
        {
            WalleType expressionType = expr.ReturnType;
            if (expressionType == WalleType.Sequence)
            {
                return true;
            }
            WalleType[] figures = new WalleType[] { WalleType.Point, WalleType.Segment, WalleType.Ray, WalleType.Line, WalleType.Circle, WalleType.Arc, WalleType.Undefined };
            foreach (var type in figures)
            {
                if (expressionType == type)
                    return true;
            }
            return false;
        }

        // comparar dos expresiones && devuelver un entero que indica su relación de orden
        internal static int CompareExpressions(IExpression a, IExpression b)
        {
            if (a.ReturnType == WalleType.Number && b.ReturnType == WalleType.Number)
            {
                double first = (double)a.Evaluate();
                double second = (double)b.Evaluate();
                return first.CompareTo(second);
            }
            else if (a.ReturnType == WalleType.Measure && b.ReturnType == WalleType.Measure)
                return ((Medidas)a.Evaluate()).CompareTo((Medidas)b.Evaluate());
            if (a.ReturnType == WalleType.Undefined)
                throw new ArgumentNullException($"{a}");
            if (b.ReturnType == WalleType.Undefined)
                throw new ArgumentNullException($"{b}");

            throw new ArgumentException($"Impossible to compare {a.ReturnType} type with {b.ReturnType} type");
        }
    }
}
