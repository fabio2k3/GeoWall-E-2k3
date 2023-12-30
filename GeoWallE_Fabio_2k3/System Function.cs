using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public delegate object System_Function(List<IExpression> args);
    public static class System_Functions_Pool
    {
        public static object Count(List<IExpression> expression) // devuelve el número de elementos en una secuencia
        {
            var sequence = (SequenceExpression)expression.ElementAt(0);

            if (sequence.ReturnType == WalleType.Undefined)
                return new UndefinedExpression();

            int count = ((Secuencias)sequence.Evaluate()).Count();

            if (count < 0)
                return new UndefinedExpression();

            return new NumberLiteralExpression(count);
        }
        public static object Print(List<IExpression> expression) // muestra el resultado de evaluar la primera expresión en la consola de la aplicación
        {
            string output = expression.ElementAt(0).Evaluate().ToString();
            System.Console.WriteLine(output);
            return new TextLiteralExpression(output);
        }
        public static object Medidas(List<IExpression> expression) // se encarga de devolver la medida de distancia entre las expresiones que toma como entrada
        {
            Point p1 = (Point)expression.ElementAt(0).Evaluate();
            Point p2 = (Point)expression.ElementAt(1).Evaluate();
            return new Medidas(p1.DistanceToPoint(p2));
        }

        public static object Line(List<IExpression> expression) // representa una linea
        {
            Point p1 = (Point)expression.ElementAt(0).Evaluate();
            Point p2 = (Point)expression.ElementAt(1).Evaluate();
            return new Line(p1, p2, "LineFromTo " + p1.Name + " " + p2.Name);
        }
        public static object Segment(List<IExpression> expression) // representa un segmento
        {
            Point p1 = (Point)expression.ElementAt(0).Evaluate();
            Point p2 = (Point)expression.ElementAt(1).Evaluate();
            return new Segment(p1, p2, "SegmentFromTo " + p1.Name + " " + p2.Name);
        }
        public static object Ray(List<IExpression> expression) // representa un rayo (semirecta)
        {
            Point p1 = (Point)expression.ElementAt(0).Evaluate();
            Point p2 = (Point)expression.ElementAt(1).Evaluate();
            return new Ray(p1, p2, "RayFromTo " + p1.Name + " " + p2.Name);
        }

        public static object Circle(List<IExpression> expression) // representa un círculo
        {
            Point p = (Point)expression.ElementAt(0).Evaluate();
            Medidas r = (Medidas)expression.ElementAt(1).Evaluate();
            return new Circle(p, r.ToFloat(), "CircleCenter " + p.Name);
        }

        public static object Arc(List<IExpression> expressions) // representa un arco
        {
            Point p1 = (Point)expressions.ElementAt(0).Evaluate();
            Point p2 = (Point)expressions.ElementAt(1).Evaluate();
            Point p3 = (Point)expressions.ElementAt(2).Evaluate();
            Medidas m = (Medidas)expressions.ElementAt(3).Evaluate();

            return new Arc(p1, p2, p3, m.ToFloat(), "ArcCenter " + p1.Name);
        }
        public static object Intersect(List<IExpression> expressions) // devuelve una instancia de la clase Secuencias que contiene los puntos de intersección entre las dos figuras
        {
            IFigure fig1 = (IFigure)expressions.ElementAt(0).Evaluate();
            IFigure fig2 = (IFigure)expressions.ElementAt(1).Evaluate();
            return new Secuencias(fig1.Intersect(fig2));
        }
    }
}
