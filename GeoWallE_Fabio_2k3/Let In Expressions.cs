using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    // representar una sentencia "let-in"
    public sealed class Let_In_Statement : IStatement, IExpression
    {
        public List<IStatement> Statements { get; private set; }
        public IExpression Body { get; private set; } // cuerpo de la expresión
        private Scope localScope;
        public WalleType ReturnType => Body.ReturnType;

        public Let_In_Statement(List<IStatement> statements, IExpression body)
        {
            this.Statements = statements;
            this.Body = body;
        }

        public void GetScope(Scope actual)
        {
            localScope = new Scope(actual);
            foreach (var item in Statements)
            {
                item.GetScope(localScope);
            }
            Body.GetScope(localScope);
        }
        public WalleType CheckSemantics() // chequear la semantica
        {
            foreach (var statement in Statements)
            {
                statement.CheckSemantics();
            }
            return Body.CheckSemantics();
        }

        // asignar valores a variables locales

        public object Evaluate() 
        {
            foreach (var item in Statements)
            {
                item.Execute();
            }
            return Body.Evaluate();
        }

        public void Execute() => Evaluate();

        public bool ConvertToBool() => (double)Evaluate() != 0;
        public override string ToString() // convertir en string 
        {
            string output = "let ";
            foreach (var item in Statements)
            {
                output += item.ToString() + "; ";
            }
            return output + $"in {Body}";
        }
    }
}
