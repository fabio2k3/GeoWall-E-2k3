using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public sealed class ConditionalExpression : IExpression, IStatement
    {
        private IExpression condition;
        private IExpression trueBody;
        private IExpression falseBody;
        public WalleType ReturnType
        {
            get
            {
                try
                {
                    return trueBody.ReturnType;
                }
                catch
                {
                    return WalleType.Undefined;
                }
            }
            private set { }
        }

        public ConditionalExpression(IExpression condition, IExpression trueBody, IExpression falseBody)
        {
            this.condition = condition;
            this.trueBody = trueBody;
            this.falseBody = falseBody;
        }

        // se encarga de llamar a cada método && pasar el objeto "actual" a cada uno 
        public void GetScope(Scope actual)
        {
            condition.GetScope(actual);
            trueBody.GetScope(actual);
            falseBody.GetScope(actual);
        }

        // verificar la semántica 
        public WalleType CheckSemantics()
        {
            condition.CheckSemantics();
            var falseReturnType = falseBody.CheckSemantics();
            var trueReturnType = trueBody.CheckSemantics();

            if (trueReturnType != falseReturnType && falseReturnType != WalleType.Undefined && trueReturnType != WalleType.Undefined)
                throw new InvalidOperationException("Then-Else branches of conditional statement must have the same return type : {this}");

            ReturnType = trueBody.ReturnType;
            return ReturnType;
        }
        public object Evaluate()
        {
            if (condition.ConvertToBool())
                return trueBody.Evaluate();

            return falseBody.Evaluate();
        }
        public void Execute() => Evaluate();
        public bool ConvertToBool() => (double)Evaluate() != 0;
        public override string ToString() => $"if({condition}) then {trueBody} else {falseBody}";
    }
}
