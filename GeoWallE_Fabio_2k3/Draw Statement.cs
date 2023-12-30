using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public sealed class Draw_Statement : IStatement, IExpression
    {
        private readonly IExpression parameter;
        private readonly List<IFigure> figures = new List<IFigure>();
        public event Action<IFigure, string> DrawThis;

        public WalleType ReturnType => WalleType.Void;

        public Draw_Statement(IExpression argument)
        {
            parameter = argument;
        }

        public void GetScope(Scope actual)
        {
            parameter.GetScope(actual);
        }
        public WalleType CheckSemantics()
        {
            parameter.CheckSemantics();
            if (!CompilatorTools.IsFigure(parameter))
                throw new ArgumentException("Draw expression parameter must be a figure");
            return WalleType.Void;
        }

        // verificar && ejecutar en correspondencia si es una secuencia
        public void Execute()
        {
            if (parameter.ReturnType == WalleType.Sequence)
            {
                Secuencias sequence = (Secuencias)parameter.Evaluate();

                foreach (IExpression element in sequence)
                {
                    try
                    {
                        DrawThis.Invoke((IFigure)element.Evaluate(), CompilatorTools.ColorPool.Peek());
                    }
                    catch { };
                }
            }
            else
            {
                figures.Add((IFigure)parameter.Evaluate());

                foreach (var item in figures)
                {
                    DrawThis.Invoke(item, CompilatorTools.ColorPool.Peek());
                }
            }
        }
        public object Evaluate()
        {
            Execute();
            return new object();
        }
        public bool ConvertToBool() => true;
        public override string ToString() => $"draw {parameter}";
    }
}
