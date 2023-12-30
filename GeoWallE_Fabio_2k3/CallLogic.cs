using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public static class CallLogic
    {
        public static SyntaxTree WorkWithCode(string code)
        {
            CompilatorTools.LoadSystemFunctions();
            CompilatorTools.ColorPool.Clear();
            CompilatorTools.ColorPool.Push("color black");
            Lexer lexer = new Lexer(code);
            Parser parser = new Parser(lexer.GetTokenList());
            SyntaxTree syntax = parser.ParseCode();
            Scope scope = new Scope();

            // obtener el alcance de cada elemento
            foreach (var item in syntax.Program)
            {
                item.GetScope(scope);
            }
            // comprobar la semántica de cada elemento
            foreach (var item in syntax.Program)
            {
                item.CheckSemantics();
            }
            return syntax;
        }
    }
}
