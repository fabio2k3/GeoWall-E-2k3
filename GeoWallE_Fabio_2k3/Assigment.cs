using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public abstract class AssigmentStatement : IStatement
    {
        protected IExpression valueExpression;
        protected Scope referencedScope;

        public AssigmentStatement(IExpression expression)
        {
            valueExpression = expression;
        }
        public virtual void GetScope(Scope actual) // asigna el valor de Scope actual a referenceScope && llama al método GetScope de la expresión valueExpression.
        {
            referencedScope = actual;
            valueExpression.GetScope(actual);
        }
        public abstract WalleType CheckSemantics(); //  verificar la semántica de la declaración && devolver un tipo de dato WalleType

        public abstract void Execute(); // ejecutar la declaración
    }

    // se asigna un valor a una variable especificada
    public sealed class SimpleAssigmentStatement : AssigmentStatement
    {
        private string variableName;

        public SimpleAssigmentStatement(string target, IExpression value) : base(value)
        {
            this.variableName = target;
        }
        public override WalleType CheckSemantics() // verifica la semántica de la asignación 
        {
            referencedScope.CreateVariableInstance(variableName, valueExpression.CheckSemantics());
            return WalleType.Void;
        }
        public override void Execute() // ejecuta la asignación
        {
            referencedScope.AssignVariable(variableName, valueExpression.Evaluate(), valueExpression.ReturnType);
        }

        public override string ToString() => $"{variableName} = {valueExpression}"; // devuelve una representación en cadena de la asignación
    }
}
