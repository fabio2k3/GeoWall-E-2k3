using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeoWallE_Fabio_2k3
{
    public interface IFigure
    {
        //Identificador de las Figuras para pintarlas
        string Name { get; set; }
        //Obtener el tipo de figura para vincularlo a la Parte Visual
        FigureType GetFigureType();
        //Trasladar una figura en el plano
        IFigure Traslate(int movX, int movY);
        //Interseccion de una figura con otra figura
        IEnumerable<IExpression> Intersect(IFigure figures);
        IEnumerable<Point> GetPoints(IFigure figures);
    }
    public interface ILine : IFigure
    {
        //Obtener la Pendiente de una Linea
        float GetPendiente();
    }

    #region Arco
    class Arc : IFigure
    {
        public Point origin;
        public Point first;
        public Point second;
        public float measure;

        public string Name { get; set; }

        public Arc(Point origin, Point extenceLeft, Point extenceRight, float measure, string name)
        {
            this.origin = origin;
            this.first = extenceLeft;
            this.second = extenceRight;
            this.measure = measure;
            Name = name;
        }

        // realiza traducción en los puntos del arco moviéndolos en la dirección especificada por los valores de X e Y
        public IFigure Traslate(int X, int Y)
        {
            Point newFirst = new Point(first.X += X, first.Y += Y, first.Name);
            Point newSecond = new Point(second.X += X, second.Y += Y, second.Name);
            Point newOrigin = new Point(origin.X += X, origin.Y += Y, origin.Name);
            return new Arc(newOrigin, newFirst, newSecond, measure, Name);
        }

        public FigureType GetFigureType()
        {
            return FigureType.Arc;
        }

        // devuelver colección de puntos que pertenecen al arco
        public IEnumerable<Point> GetPoints(IFigure figures) 
        {
            Circle aux = new Circle(origin, measure, Name);
            IEnumerable<Point> pointsIntersec = aux.GetPoints(figures);
            Ray start = new Ray(origin, first, "Start");
            Ray end = new Ray(origin, second, "End");
            float startAngle = start.GetAngle();
            float endAngle = end.GetAngle();
            startAngle = startAngle < 0 ? 360 + startAngle : startAngle;
            endAngle = endAngle < 0 ? 360 + endAngle : endAngle;

            foreach (var item in pointsIntersec)
            {
                Ray current = new Ray(origin, item, "Current");
                float currentAngle = current.GetAngle();
                currentAngle = currentAngle < 0 ? 360 + currentAngle : currentAngle;


                if (startAngle < endAngle && currentAngle > startAngle && currentAngle < endAngle) 
                { 
                    yield return item; 
                }
                else if (currentAngle < endAngle || currentAngle > startAngle) 
                { 
                    yield return item; 
                }
            }
        }

        public IEnumerable<IExpression> Intersect(IFigure figures) // obtener los puntos de intersecto
        {
            foreach (var item in GetPoints(figures))
            {
                yield return new PointExpression(item.Name, item.X, item.Y);
            }
        }
    }
    #endregion

    #region Circulo
    public class Circle : IFigure
    {
        public Circle(Point center, float ratio, string name)
        {
            this.Center = center;
            this.Ratio = ratio;
            this.Name = name;
        }

        public Point Center { get; private set; }
        public float Ratio { get; private set; }
        public string Name { get; set; }

        public FigureType GetFigureType()
        {
            return FigureType.Circle;
        }

        // obtener la colección de puntos de la circunferencia
        public IEnumerable<Point> GetPoints(IFigure figures)
        {
            if (figures is Point)
            {
                Point P = (Point)figures;
                float suposs = ((P.X - Center.X) * (P.X - Center.X)) + ((P.Y - Center.Y) * (P.Y - Center.Y));
                if (suposs == Ratio * Ratio)
                {
                    yield return P;
                }
            }
            if (figures is Line || figures is Segment || figures is Ray)
            {
                foreach (var item in figures.GetPoints(this))
                {
                    yield return item;
                }
            }
            if (figures is Circle)
            {
                Circle other = (Circle)figures;
                double distanceCenters = Math.Sqrt(Math.Pow(Center.X - other.Center.X, 2) + Math.Pow(Center.Y - other.Center.Y, 2));
                if (distanceCenters > Ratio + other.Ratio || distanceCenters < Math.Abs(Ratio - other.Ratio)) { yield break; }

                double d = distanceCenters;
                double a = (Math.Pow(Ratio, 2) - Math.Pow(other.Ratio, 2) + Math.Pow(d, 2)) / (d * 2);
                double h = Math.Sqrt(Math.Pow(Ratio, 2) - Math.Pow(a, 2));

                double x3 = Center.X + a * (other.Center.X - Center.X) / d;
                double y3 = Center.Y + a * (other.Center.Y - Center.Y) / d;

                double x1 = x3 + h * (other.Center.Y - Center.Y) / d;
                double y1 = y3 - h * (other.Center.X - Center.X) / d;

                double x2 = x3 - h * (other.Center.Y - Center.Y) / d;
                double y2 = y3 + h * (other.Center.X - Center.X) / d;

                yield return new Point((float)x1, (float)y1, "IC1");
                yield return new Point((float)x2, (float)y2, "IC2");
            }
            if (figures is Arc)
            {
                Arc arc = (Arc)figures;
                IEnumerable<Point> points = arc.GetPoints(this);
                foreach (var item in points)
                {
                    yield return item;
                }
            }
            yield break;
        }

        // hallar las intersecciones
        public IEnumerable<IExpression> Intersect(IFigure figures)
        {
            foreach (var item in GetPoints(figures))
            {
                yield return new PointExpression(item.Name, item.X, item.Y);
            }
        }

        // realiza traducción en los puntos de la circunferencia en las nuevas coordenadas
        public IFigure Traslate(int movX, int movY)
        {
            Point newCenter = new Point(Center.X + movX, Center.Y + movY, Center.Name);
            this.Center = newCenter;
            return this;
        }
    }
    #endregion

    #region Linea
    public class Line : ILine
    {
        public Line(Point p1, Point p2, string name)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.Name = name;
        }

        public Point p1 { get; private set; }
        public Point p2 { get; private set; }
        public string Name { get; set; }

        public FigureType GetFigureType()
        {
            return FigureType.Line;
        }
        // realizar traducci+on en los puntos de la recta en las nuevas coordenadas
        public virtual IFigure Traslate(int movX, int movY)
        {
            Point newP1 = new Point(p1.X + movX, p1.Y + movY, p1.Name);
            Point newP2 = new Point(p2.X + movX, p2.Y + movY, p2.Name);
            this.p1 = newP1;
            this.p2 = newP2;
            return this;
        }
        // hallar puntos de intersección
        public IEnumerable<IExpression> Intersect(IFigure figures)
        {
            foreach (var item in GetPoints(figures))
            {
                yield return new PointExpression(item.Name, item.X, item.Y);
            }
        }
        // obtener colección de puntos 
        public IEnumerable<Point> GetPoints(IFigure figures)
        {
            double m = GetPendiente();
            double n = p1.Y - m * p1.X;
            if (figures is Point)
            {
                Point p = (Point)figures;
                if (p.Y == m * p.X + n)
                {
                    yield return new Point(p.X, p.Y, "P");
                    yield break;
                }
            }
            if (figures is Line)
            {
                Line l = (Line)figures;
                double mL = l.GetPendiente();
                double nL = l.p1.Y - mL * l.p1.X;
                if (mL == m)
                {
                    while (nL == n)
                    {
                        yield return new Point(p1.X += 1, (float)(m * p1.X + n), "P");
                    }
                    yield break;
                }
                else
                {
                    float newX = (float)((nL - n) / (m - mL));
                    yield return new Point(newX, (float)(mL * newX + nL), "P");
                }
            }
            if (figures is Ray)
            {
                Ray r = (Ray)figures;
                Line rayLine = new Line(r.p1, r.p2, r.Name);
                IEnumerable<Point> pointsIntersect = GetPoints(rayLine);
                (float, float) vector = r.GetVector();
                foreach (var item in pointsIntersect)
                {
                    (float, float) vector2 = (item.Y - r.p1.Y, item.X - r.p1.X);
                    if ((Math.Sign(vector.Item1) == Math.Sign(vector2.Item1)) || (Math.Sign(vector.Item2) == Math.Sign(vector2.Item2)))
                    {
                        yield return item;
                    }
                }
                yield break;
            }
            if (figures is Segment)
            {
                Segment s = (Segment)figures;
                Line segmentLine = new Line(s.p1, s.p2, s.Name);
                IEnumerable<Point> pointsIntersect = GetPoints(segmentLine);
                foreach (var item in pointsIntersect)
                {
                    if (item.X < Math.Max(s.p1.X, s.p2.X) && item.X > Math.Min(s.p1.X, s.p2.X)) yield return item;
                }
                yield break;
            }
            if (figures is Circle)
            {
                Circle c = (Circle)figures;
                double p = n - c.Center.Y;
                double A = m * m + 1;
                double B = (2 * p * m) - 2 * c.Center.X;
                double C = (c.Center.X * c.Center.X) + (p * p) - (c.Ratio * c.Ratio);

                double D = (B * B) - (4 * A * C);
                if (D > 0)
                {
                    double x1 = (-B + Math.Sqrt(D)) / (2 * A);
                    double x2 = (-B - Math.Sqrt(D)) / (2 * A);
                    double y1 = m * x1 + n;
                    double y2 = m * x2 + n;
                    yield return new Point((float)x1, (float)y1, "IC1");
                    yield return new Point((float)x2, (float)y2, "IC2");
                }
                else if (D < 0)
                {
                    yield break;
                }
                else
                {
                    float X = (float)(-B / (A * 2));
                    yield return new Point(X, (float)(m * X + n), "IC1");
                }
            }
            if (figures is Arc)
            {
                Arc arc = (Arc)figures;
                IEnumerable<Point> points = arc.GetPoints(this);
                foreach (var item in points)
                {
                    yield return item;
                }
            }
            yield break;
        }

        public virtual float GetPendiente() // obtener la pendiente
        {
            return (p2.Y - p1.Y) / (p2.X - p1.X);
        }
    }
    #endregion

    #region Punto
    public class Point : IFigure
    {
        public Point(float x, float y, string name)
        {
            X = x;
            Y = y;
            this.Name = name;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }

        public FigureType GetFigureType()
        {
            return FigureType.Point;
        }
        // obtener intersectos
        public IEnumerable<IExpression> Intersect(IFigure figures)
        {
            return figures.Intersect(this);
        }
        // realizar la traducción del punto en las nuevas coordenadas
        public IFigure Traslate(int movX, int movY)
        {
            return new Point(this.X + movX, this.Y + movY, this.Name);
        }
        // obtener colección de puntos
        public IEnumerable<Point> GetPoints(IFigure figure) => new List<Point>() { this };
        internal double DistanceToPoint(Point p2)
        {
            return Math.Sqrt(Math.Pow(this.X - p2.X, 2) + Math.Pow(this.Y - p2.Y, 2));
        }
    }
    #endregion

    #region Rayo
    public class Ray : IFigure
    {
        public Point p1;
        public Point p2;
        public string Name { get; set; }

        public Ray(Point p1, Point p2, string name)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.Name = name;
        }
        // obtener colección de puntos del Rayo
        public IEnumerable<Point> GetPoints(IFigure figures)
        {
            Line aux = new Line(p1, p2, "aux");
            IEnumerable<Point> points = aux.GetPoints(figures);
            (float, float) vector = GetVector();
            foreach (var item in points)
            {
                (float, float) vector2 = (item.Y - p1.Y, item.X - p1.X);
                if ((Math.Sign(vector.Item1) == Math.Sign(vector2.Item1)) || (Math.Sign(vector.Item2) == Math.Sign(vector2.Item2)))
                {
                    yield return item;
                }
            }
        }
        public float GetPendiente() // obtener pendiente
        {
            return (p2.Y - p1.Y) / (p2.X - p1.X);
        }
        public (float, float) GetVector() // obtener el vector
        {
            return (p2.X - p1.X, p2.Y - p1.Y);
        }
        public FigureType GetFigureType()
        {
            return FigureType.Ray;
        }
        public float GetAngle() // obtener el angulo
        {
            float m = GetPendiente();
            float angle = (float)(Math.Atan(m) * 180 / Math.PI);
            if (m >= 0)
            {
                angle = p1.Y > p2.Y ? -180 + angle : angle;
            }
            else
            {
                angle = p1.Y > p2.Y ? angle : 180 + angle;
            }
            return angle;
        }

        public IFigure Traslate(int movX, int movY) // tradcción del rayo en las nuevas coordenadas
        {
            Point newP1 = new Point(p1.X + movX, p1.Y + movY, p1.Name);
            Point newP2 = new Point(p2.X + movX, p2.Y + movY, p2.Name);
            this.p1 = newP1;
            this.p2 = newP2;
            return this;
        }

        public IEnumerable<IExpression> Intersect(IFigure figures) // hallar los interctos
        {
            foreach (var item in GetPoints(figures))
            {
                yield return new PointExpression(item.Name, item.X, item.Y);
            }
        }
    }
    #endregion

    #region Segmento
    public class Segment : IFigure
    {
        public Point p1;
        public Point p2;
        public string Name { get; set; }
        public Segment(Point p1, Point p2, string name)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.Name = name;
        }
        public IEnumerable<Point> GetPoints(IFigure figures) // obtener colección de puntos
        {
            Line aux = new Line(p1, p2, "aux");
            IEnumerable<Point> points = aux.GetPoints(figures);
            foreach (var item in points)
            {
                if (item.X < Math.Max(p1.X, p2.X) && item.X > Math.Min(p1.X, p2.X)) yield return item;
            }
        }
        public float GetPendiente() // obtener la pendiente
        {
            return (p2.Y - p1.Y) / (p2.X - p1.X);
        }
        public FigureType GetFigureType()
        {
            return FigureType.Segment;
        }
        // realiza traducción del segmento en las nuevas coordenadas
        public IFigure Traslate(int movX, int movY)
        {
            Point newP1 = new Point(p1.X + movX, p1.Y + movY, p1.Name);
            Point newP2 = new Point(p2.X + movX, p2.Y + movY, p2.Name);
            this.p1 = newP1;
            this.p2 = newP2;
            return this;
        }
        // hallar intersectos
        public IEnumerable<IExpression> Intersect(IFigure figures)
        {
            foreach (var item in GetPoints(figures))
            {
                yield return new PointExpression(item.Name, item.X, item.Y);
            }
        }
    }
    #endregion
}
