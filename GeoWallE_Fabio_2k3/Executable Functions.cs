using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GeoWallE_Fabio_2k3
{
    public abstract class Executable_Function : IStatement
    {
        public string Name { get; private set; }
        public abstract int Count { get; }
        public abstract WalleType ReturnType { get; }

        public Executable_Function(string name)
        {
            Name = name;
        }
        public virtual void GetScope(Scope actual) { }
        public virtual WalleType CheckSemantics() => ReturnType; // devuelve el valor de la propiedad ReturnType.
        public virtual void Execute() { }
        public virtual void ResetScope() { }
        public abstract object Run(List<IExpression> arguments);
        public bool Match(string name, int count) // Comparar el nombre de la función con el nombre pasado como argumento y el contador con el valor de la propiedad Count
        {
            return Name == name && Count == count;
        }
    }
}
