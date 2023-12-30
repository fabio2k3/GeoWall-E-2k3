using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;

namespace GeoWallE_Fabio_2k3
{
    public sealed class Declared_Function_Expression : Executable_Function
    {
        private List<string> parametersName;
        private IExpression Body;
        private Scope localScope;

        public override int Count => parametersName.Count;
        public override WalleType ReturnType => Body.ReturnType;

        public Declared_Function_Expression(string name, List<string> parameters, IExpression body) : base(name)
        {
            parametersName = parameters;
            Body = body;
        }
        public override void GetScope(Scope actual)
        {
            localScope = new Scope(actual);
            Body.GetScope(localScope);
            CompilatorTools.AddFunction(this);
        }

        // crear instancias de variables con nombres de parámetros 
        public override WalleType CheckSemantics()
        {
            foreach (var parameter in parametersName)
            {
                localScope.CreateVariableInstance(parameter, WalleType.Undefined);
            }
            return Body.CheckSemantics();
        }

        public override void Execute() { }

        // asignar a cada variable local el valor 
        public override object Run(List<IExpression> arguments)
        {
            for (int i = 0; i < parametersName.Count; i++)
            {
                localScope.AssignVariable(parametersName.ElementAt(i), arguments.ElementAt(i).Evaluate(), arguments.ElementAt(i).ReturnType);
            }
            System.Console.WriteLine(Body.ReturnType);
            return Body.Evaluate();
        }

        // eliminar los elementos de parametersName del ámbito local 
        public override void ResetScope()
        {
            foreach (var item in parametersName)
            {
                localScope.RemoveLast(item);
            }
        }

        //public override string ToString()
        //{
        //    string output = Name + "(";
        //    for (int i = 0; i < parametersName.Count; i++)
        //    {
        //        output += parametersName.ElementAt(i).ToString();
        //        if (i != parametersName.Count - 1) { output += ", "; }
        //    }
        //    return output + $") = {Body}";
        //}
    }
}

