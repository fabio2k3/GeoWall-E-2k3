using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public class Function_Call_Expression : IExpression, IStatement
    {
        public string Name { get; private set; }

        public WalleType ReturnType => functionImplementation.ReturnType;
        private Executable_Function functionImplementation;

        private List<IExpression> args;

        public Function_Call_Expression(string name, List<IExpression> args)
        {
            Name = name;
            this.args = args;
        }
        public void GetScope(Scope actual) // recorre cada expresión de argumento y llama al método GetScope de cada una
        {
            foreach (var item in args) 
            {
                item.GetScope(actual); 
            }
        }
        public WalleType CheckSemantics() // busca la implementación de la función && verifica la semántica de cada expresión
        {
            functionImplementation = CompilatorTools.SearchFunction(Name, args.Count);
            foreach (var expression in args)
            {
                expression.CheckSemantics();
            }
            return ReturnType;
        }
        public object Evaluate() // implementa la función && devuelve su valor
        {
            var result = functionImplementation.Run(args);
            functionImplementation.ResetScope();
            return result;
        }
        public void Execute() => Evaluate();

        public bool ConvertToBool() => (double)Evaluate() != 0; // convierte el resultado de la función llamada a un valor booleano.
        public override string ToString()
        {
            string output = $"{Name}(";
            for (int i = 0; i < args.Count; i++)
            {
                output += args[i].ToString();
                if (i != args.Count - 1) { output += ","; }
            }
            return $"{output})";
        }
    }
}
