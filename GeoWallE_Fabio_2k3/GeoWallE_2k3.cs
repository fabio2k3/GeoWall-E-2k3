namespace GeoWallE_Fabio_2k3
{
    public partial class GeoWallE_2k3 : Form
    {
        public GeoWallE_2k3()
        {
            InitializeComponent();
        }
        private List<IFigure> Figures = new List<IFigure>();

        // Compile & Run
        private void button1_Click(object sender, EventArgs e)
        {
            rtb_Error.Text = "";
            if (rtb_Code.Text == "") return;
            //Reinicia el lienzo
            pctBox_Draw.Refresh();
            //Guarda la informacion del TextBox donde se escribe el codigo
            string code = rtb_Code.Text;
            //Hacemos una nueva lista 
            Figures = new List<IFigure>();

            //Inicia el proceso de Tokenizacion, Parseo y Chequeo Semantico del codigo
            try
            {
                SyntaxTree syntax = GeoWallE_Fabio_2k3.CallLogic.WorkWithCode(code);
                foreach (var item in syntax.Program)
                {
                    if (item is Draw_Statement)
                    {
                        var test = (Draw_Statement)item;
                        test.DrawThis += DrawFigure;
                    }
                    item.Execute();
                    Thread.Sleep(100);
                }
            }
            catch (Exception E)
            {
                rtb_Error.Text = E.Message;
            }
        }

        #region Dibujar
        public void DrawFigure(IFigure figure, string color)
        {
            /*Este método es el encargado de dibujar las figuras, llamando al método correspondiente.
            Utiliza herramientas de Windows Form, el lector podrá darse cuenta de que hace cada método
            solamente leyendo el codigo*/
            Graphics graphics = pctBox_Draw.CreateGraphics();
            Color actualColor = GetColor(color);

            /*Antes de agregar una figura a la lista preguntar si no existe ninguna con su mismo nombre
            Esta línea ayuda a la hora de trasladar las figuras en el lienzo*/
            if (!(Figures.Any(x => x.Name == figure.Name))) { Figures.Add(figure); }
            FigureType thisType = figure.GetFigureType();
            //Chequear el tipo de figura para llamar al metodo correspondientes
            if (thisType == FigureType.Point)
            {
                GeoWallE_Fabio_2k3.Point p1 = (GeoWallE_Fabio_2k3.Point)figure;
                MyDrawPoint(p1, graphics);
                return;
            }
            if (thisType == FigureType.Circle)
            {
                Circle c1 = (Circle)figure;
                MyDrawCircle(c1, graphics, actualColor);
                return;
            }
            if (thisType == FigureType.Segment)
            {
                Segment l1 = (Segment)figure;
                MyDrawSegment(l1, graphics, actualColor);
                return;
            }
            if (thisType == FigureType.Line)
            {
                Line l1 = (Line)figure;
                MyDrawLine(l1, graphics, actualColor);
                return;
            }
            if (thisType == FigureType.Ray)
            {
                Ray l1 = (Ray)figure;
                MyDrawRay(l1, graphics, actualColor);
                return;
            }
            if (thisType == FigureType.Arc)
            {
                Arc a1 = (Arc)figure;
                MyDrawArc(a1, graphics, actualColor);
                return;
            }
        }

        private void MyDrawPoint(GeoWallE_Fabio_2k3.Point point, Graphics graphics)
        {
            graphics.FillEllipse(Brushes.Black, point.X - 3, point.Y - 3, 6, 6);
            graphics.DrawString(point.Name, new Font("Arial", 10), Brushes.Black, point.X, point.Y);
        }

        private void MyDrawCircle(Circle circle, Graphics graphics, Color color)
        {
            graphics.DrawEllipse(new Pen(color), circle.Center.X - circle.Ratio, circle.Center.Y - circle.Ratio, (float)circle.Ratio * 2, (float)circle.Ratio * 2);
            graphics.FillEllipse(Brushes.Black, circle.Center.X - 3, circle.Center.Y - 3, 6, 6);
        }

        private void MyDrawSegment(Segment line, Graphics graphics, Color color)
        {
            graphics.DrawLine(new Pen(color), line.p1.X, line.p1.Y, line.p2.X, line.p2.Y);
            graphics.FillEllipse(Brushes.Black, line.p1.X - 3, line.p1.Y - 3, 6, 6);
            graphics.FillEllipse(Brushes.Black, line.p2.X - 3, line.p2.Y - 3, 6, 6);
        }

        private void MyDrawLine(Line line, Graphics graphics, Color color)
        {
            float m = line.GetPendiente();
            float n = line.p1.Y - m * line.p1.X;
            (GeoWallE_Fabio_2k3.Point, GeoWallE_Fabio_2k3.Point) interceps = GetIntersepts(m, n);
            graphics.DrawLine(new Pen(color), interceps.Item1.X, interceps.Item1.Y, interceps.Item2.X, interceps.Item2.Y);
            graphics.FillEllipse(Brushes.Black, line.p1.X - 3, line.p1.Y - 3, 6, 6);
            graphics.FillEllipse(Brushes.Black, line.p2.X - 3, line.p2.Y - 3, 6, 6);
        }

        private void MyDrawRay(Ray line, Graphics graphics, Color color)
        {

            graphics.DrawLine(new Pen(color), line.p1.X, line.p1.Y, line.p2.X, line.p2.Y);
            float m = line.GetPendiente();
            float n = line.p1.Y - m * line.p1.X;
            (GeoWallE_Fabio_2k3.Point, GeoWallE_Fabio_2k3.Point) interceps = GetIntersepts(m, n);

            (float, float) vector1 = (line.p2.X - line.p1.X, line.p2.Y - line.p2.X);

            (float, float) vector2 = (interceps.Item1.X - line.p1.X, interceps.Item1.Y - line.p1.Y);

            (float, float) vector3 = (interceps.Item2.X - line.p1.X, interceps.Item2.Y - line.p1.Y);

            if (vector1.Item1 > 0 && vector2.Item1 > 0 || vector1.Item1 < 0 && vector2.Item1 < 0)
            {
                graphics.DrawLine(new Pen(color), line.p1.X, line.p1.Y, interceps.Item1.X, interceps.Item1.Y);
            }

            else if (vector1.Item1 > 0 && vector3.Item1 > 0 || vector1.Item1 < 0 && vector3.Item1 < 0)
            {
                graphics.DrawLine(new Pen(color), line.p1.X, line.p1.Y, interceps.Item2.X, interceps.Item2.Y);
            }
            graphics.FillEllipse(Brushes.Black, line.p1.X - 3, line.p1.Y - 3, 6, 6);
            graphics.FillEllipse(Brushes.Black, line.p2.X - 3, line.p2.Y - 3, 6, 6);
        }
        private void MyDrawArc(Arc arc, Graphics graphics, Color color)
        {
            graphics.FillEllipse(Brushes.Black, arc.origin.X - 3, arc.origin.Y - 3, 6, 6);
            graphics.FillEllipse(Brushes.Black, arc.first.X - 3, arc.first.Y - 3, 6, 6);
            graphics.FillEllipse(Brushes.Black, arc.second.X - 3, arc.second.Y - 3, 6, 6);

            float startAngle = GetAngle(arc.origin, arc.second);
            float endAngle = GetAngle(arc.origin, arc.first);

            float possitiveStart = Math.Sign(startAngle) * startAngle;
            float possitiveEnd = Math.Sign(endAngle) * endAngle;
            float sweepAngle = 0;

            if (Math.Sign(startAngle) == Math.Sign(endAngle))
            {
                if (startAngle < 0)
                    sweepAngle = possitiveStart > possitiveEnd ? possitiveStart - possitiveEnd : 360 - possitiveEnd + possitiveStart;
                else
                    sweepAngle = possitiveStart > possitiveEnd ? 360 - possitiveStart + possitiveEnd : possitiveEnd - possitiveStart;
            }
            else
            {
                sweepAngle = Math.Sign(endAngle) > 0 ? possitiveEnd + possitiveStart : 360 - possitiveEnd - possitiveStart;
            }

            graphics.DrawArc(new Pen(color), arc.origin.X - arc.measure, arc.origin.Y - arc.measure, arc.measure * 2, arc.measure * 2, startAngle, sweepAngle);
        }
        private Color GetColor(string color)
        {
            switch (color)
            {
                case "red": return Color.Red;
                case "blue": return Color.Blue;
                case "yellow": return Color.Yellow;
                case "green": return Color.Green;
                case "cyan": return Color.Cyan;
                case "magenta": return Color.Magenta;
                case "white": return Color.White;
                case "gray": return Color.Gray;
                default: return Color.Black;
            }
        }
        #endregion

        #region Metodos Necesarios Para el Graficado
        private float GetPendiente(GeoWallE_Fabio_2k3.Point p1, GeoWallE_Fabio_2k3.Point p2)
        {
            return ((p2.Y - p1.Y) / (p2.X - p1.X));
        }
        private (GeoWallE_Fabio_2k3.Point, GeoWallE_Fabio_2k3.Point) GetIntersepts(float m, float n)
        {
            float intercepUP = -(n / m);
            if (intercepUP < 0)
            {
                return (new GeoWallE_Fabio_2k3.Point(0, n, ""), new GeoWallE_Fabio_2k3.Point((pctBox_Draw.Height - 1 - n) / m, pctBox_Draw.Height - 1, ""));
            }
            if (n < 0)
            {
                return (new GeoWallE_Fabio_2k3.Point(intercepUP, 0, ""), new GeoWallE_Fabio_2k3.Point(pctBox_Draw.Width - 1, m * pctBox_Draw.Width - 1 + n, ""));
            }
            return (new GeoWallE_Fabio_2k3.Point(intercepUP, 0, ""), new GeoWallE_Fabio_2k3.Point(0, n, ""));
        }
        private float GetAngle(GeoWallE_Fabio_2k3.Point p1, GeoWallE_Fabio_2k3.Point p2)
        {
            float m = GetPendiente(p1, p2);
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
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}