using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public sealed class PredefinedFunction : Executable_Function // definir una función predefinida
    {
        public override int Count { get; }
        public override WalleType ReturnType { get; }
        private Func<List<IExpression>, object> function;
        
        public PredefinedFunction(string name, int count, WalleType returnType, Func<List<IExpression>, object> function) : base(name)
        {
            Count = count;
            ReturnType = returnType;
            this.function = function;
        }
        public override object Run(List<IExpression> arguments) => function(arguments);
    }
}
