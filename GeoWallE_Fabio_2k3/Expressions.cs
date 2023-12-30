using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public interface IExpression : INode
    {
        WalleType ReturnType { get; }
        object Evaluate();
        bool ConvertToBool();
    }

    #region Unary Expressions
    public abstract class UnaryOperatorExpression : IExpression // representar y utilizar las expresiones de operadores unarios
    {
        protected IExpression Operand;
        public WalleType ReturnType => WalleType.Number;
        public string OperatorSymbol { get; private set; }

        public UnaryOperatorExpression(IExpression operand, string operatorSymbol)
        {
            Operand = operand;
            OperatorSymbol = operatorSymbol.ToString();
        }

        public void GetScope(Scope actual) { Operand.GetScope(actual); }
        public abstract WalleType CheckSemantics();
        public abstract object Evaluate();
        public abstract bool ConvertToBool();
        public override string ToString() => $"{OperatorSymbol} {Operand}";
    }

    public sealed class NegativeOperatorExpression : UnaryOperatorExpression // representar expresion de operador negativo
    {
        public NegativeOperatorExpression(IExpression operand, string operatorSymbol) : base(operand, operatorSymbol) { }
        // solo se puede utilizar con expresiones de tipo Number o Measure
        public override WalleType CheckSemantics()
        {
            WalleType operandType = Operand.CheckSemantics();

            if (operandType != WalleType.Number && operandType != WalleType.Measure && operandType != WalleType.Undefined)
                throw new InvalidOperationException($"Cannot apply {OperatorSymbol} to {operandType} type.");
            return WalleType.Number;
        }
        public override object Evaluate()
        {
            if (Operand.ReturnType == WalleType.Measure)
                return -(Medidas)Operand.Evaluate();

            return -(double)Operand.Evaluate();
        }

        public override bool ConvertToBool() => (double)Evaluate() != 0;
    }

    public sealed class NotOperatorExpression : UnaryOperatorExpression // representación del operador "not"
    {
        public NotOperatorExpression(IExpression operand, string operatorSymbol) : base(operand, operatorSymbol) { }
        public override WalleType CheckSemantics() => WalleType.Number;
        public override object Evaluate()
        {
            if (!Operand.ConvertToBool())
                return 1.0;
            return 0.0;
        }
        public override bool ConvertToBool() => !Operand.ConvertToBool();
    }
    #endregion

    #region Variable Expressions
    public sealed class VariableExpression : IExpression
    {
        public string Name; // nombre variable
        private Scope referencedScope; // alcance en que se encuentra
        public WalleType ReturnType => (referencedScope != null) ? referencedScope.GetVariableType(Name) : WalleType.Undefined; // tipo de la variable

        public VariableExpression(Token id)
        {
            Name = id.Value;
        }

        public void GetScope(Scope actual)
        {
            referencedScope = actual;
        }
        public WalleType CheckSemantics()
        {
            return referencedScope.GetVariableType(Name);
        }

        public object Evaluate() // devolver su valor a partir del alcance
        {
            return referencedScope.GetVariableValue(Name);
        }
        public bool ConvertToBool() // convertir la variable a un valor booleano
        {
            if (ReturnType == WalleType.Sequence)
            {
                var SequenceExp = new SequenceExpression((Secuencias)Evaluate());
                return SequenceExp.ConvertToBool();
            }
            return (double)Evaluate() != 0;
        }
        public override string ToString() => Name;
    }
    #endregion

    #region Point Expressions
    public sealed class PointExpression : IExpression // caracteristicas puntos
    {
        Point point;
        string name;
        Scope referencedScope;
        public WalleType ReturnType => WalleType.Point;
        float X;
        float Y;

        public PointExpression(string name, float X = float.PositiveInfinity, float Y = float.PositiveInfinity)
        {
            this.name = name;
            this.X = X;
            this.Y = Y;
        }
        public void GetScope(Scope actual)
        {
            referencedScope = new Scope(actual);
        }
        public WalleType CheckSemantics() => WalleType.Point;
        public object Evaluate() // asignar coordenadas
        {
            Random r = new Random();
            if (X.Equals(float.PositiveInfinity) && Y.Equals(float.PositiveInfinity))
                point = new Point(r.Next(0, 520), r.Next(0, 380), name);
            else
                point = new Point(X, Y, name);
            return point;
        }
        public bool ConvertToBool() => true;
    }
    #endregion

    #region Line Expressions
    public class LineExpression : IExpression
    {
        public Line line { get; private set; }
        public string name; // nombre
        public TokenType lineTypes;
        public WalleType ReturnType => WalleType.Line;

        public LineExpression(string name, TokenType lineTypes)
        {
            this.name = name;
            this.lineTypes = lineTypes;
        }
        public void GetScope(Scope actual) { }
        public WalleType CheckSemantics() => WalleType.Line;

        public object Evaluate() // generar dos puntos aleatorios y luego crear el tipo de línea correspondiente
        {
            Random r = new Random();
            Point p1 = new Point(r.Next(0, 300), r.Next(0, 300), name + "p1");
            Point p2 = new Point(r.Next(0, 300), r.Next(0, 300), name + "p2");
            if (lineTypes == TokenType.LineKwToken)
            {
                return new Line(p1, p2, name);
            }
            else if (lineTypes == TokenType.RayKwToken)
            {
                return new Ray(p1, p2, name);
            }
            else if (lineTypes == TokenType.SegmentKwToken)
            {
                return new Segment(p1, p2, name);
            }
            throw new InvalidCastException();
        }
        public bool ConvertToBool() => true;
    }
    #endregion

    #region Circle Expressions
    public sealed class CircleExpression : IExpression
    {
        string name;// nombre
        Circle circle;
        public WalleType ReturnType => WalleType.Circle;

        public CircleExpression(string name)
        {
            this.name = name;
        }
        public void GetScope(Scope actual) { }

        public WalleType CheckSemantics() => WalleType.Circle;

        public object Evaluate() // crear una circunferencia aleatoria
        {
            Random r = new Random();
            circle = new Circle(new Point(r.Next(0, 900), r.Next(0, 570), "center"), r.Next(50, 100), name);
            return circle;
        }
        public bool ConvertToBool() => true;
    }
    #endregion

    #region Logic Operators
    public abstract class LogicOperatorExpression : BinaryOperatorExpression
    {
        public LogicOperatorExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override WalleType CheckSemantics()
        {
            left.CheckSemantics();
            right.CheckSemantics();
            ReturnType = WalleType.Number;
            return ReturnType;
        }
    }

    public sealed class AndExpression : LogicOperatorExpression
    {
        public AndExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }
        public override object Evaluate() // chequear si ambas expresiones se pueden cobvertir en True
        {
            if (left.ConvertToBool() && right.ConvertToBool())
            {
                return 1.0;
            }
            return 0.0;
        }
    }

    public sealed class OrExpression : LogicOperatorExpression
    {
        public OrExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }
        public override object Evaluate() // chequear si ambas expresiones se pueden cobvertir en True
        {
            if (left.ConvertToBool() || right.ConvertToBool())
            {
                return 1.0;
            }
            return 0.0;
        }
    }
    #endregion

    #region Comparative Expressiones

    // clases encargadas de realizar los operadores de COMPARACION
    public abstract class ComparativeExpression : BinaryOperatorExpression
    {
        protected ComparativeExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol)
        {
            validTypes[(WalleType.Measure, WalleType.Measure)] = WalleType.Number;
        }
    }
    public sealed class GreaterExpression : ComparativeExpression // mayor ">"
    {
        public GreaterExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override object Evaluate()  
        {
            if (CompilatorTools.CompareExpressions(left, right) > 0)
                return 1.0;
            return 0.0;
        }
    }

    public sealed class GreaterEqualsExpression : ArithmeticExpression // mayor igual ">="
    {
        public GreaterEqualsExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override object Evaluate()
        {
            if (CompilatorTools.CompareExpressions(left, right) >= 0)
                return 1.0;
            return 0.0;
        }
    }

    public sealed class LessExpression : ArithmeticExpression // menor "<"
    {
        public LessExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override object Evaluate()
        {
            if (CompilatorTools.CompareExpressions(left, right) < 0)
                return 1.0;
            return 0.0;
        }
    }

    public sealed class LessEqualsExpression : ArithmeticExpression // menor igual "<="
    {
        public LessEqualsExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override object Evaluate()
        {
            if (CompilatorTools.CompareExpressions(left, right) <= 0)
                return 1.0;
            return 0.0;
        }
    }

    public sealed class EqualityExpression : ComparativeExpression // igualdad "=="
    {
        public EqualityExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override object Evaluate()
        {
            if (CompilatorTools.CompareExpressions(left, right) == 0)
            {
                return 1.0;
            }
            return 0.0;
        }
    }

    public sealed class NotEqualityExpression : ComparativeExpression // desigualdad "!="
    {
        public NotEqualityExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }

        public override object Evaluate()
        {
            if (CompilatorTools.CompareExpressions(left, right) != 0)
                return 1.0;
            return 0.0;
        }
    }
    #endregion

    #region Binary Expressions
    public abstract class BinaryOperatorExpression : IExpression
    {
        protected IExpression left;
        protected IExpression right;
        protected string OperatorSymbol { get; private set; }
        public WalleType ReturnType { get; protected set; }

        // almacena los pares de tipos de datos válidos para la expresión binaria y su tipo de retorno correspondiente
        protected Dictionary<(WalleType, WalleType), WalleType> validTypes; 

        protected BinaryOperatorExpression(IExpression left, IExpression right, string operatorSymbol)
        {
            this.left = left;
            this.right = right;

            OperatorSymbol = operatorSymbol;
            ReturnType = WalleType.Undefined;

            validTypes = new Dictionary<(WalleType, WalleType), WalleType>();
            validTypes[(WalleType.Number, WalleType.Number)] = WalleType.Number;
        }
        public void GetScope(Scope actual)
        {
            left.GetScope(actual);
            right.GetScope(actual);
        }
        public virtual WalleType CheckSemantics() // verificar la semántica de la expresión binaria 
        {
            var leftType = left.CheckSemantics();
            var rightType = right.CheckSemantics();

            // verifica si alguno de los tipos es indefinido y asigna el tipo no indefinido al atributo "ReturnType"
            if (leftType == WalleType.Undefined)
                ReturnType = rightType;

            else if (rightType == WalleType.Undefined)
                ReturnType = leftType;

            // si los tipos de datos son válidos según el diccionario "validTypes", también asigna el tipo correspondiente al atributo "ReturnType"
            else if (validTypes.TryGetValue((leftType, rightType), out var type))
                ReturnType = type;

            else
                throw new InvalidOperationException($"Operator \"{OperatorSymbol}\" cannot be applied to : WalleType.{leftType} and WalleType.{rightType} at \n {this} ");
            return ReturnType;
        }
        public abstract object Evaluate();
        public bool ConvertToBool() => (double)Evaluate() != 0;
        public override string ToString() => $"{left} {OperatorSymbol} {right}";
    }
    #endregion

    #region ArithMetic Expression

    // Operaciones Aritmeticas
    public abstract class ArithmeticExpression : BinaryOperatorExpression
    {
        public ArithmeticExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol) { }
    }

    public sealed class SumExpression : ArithmeticExpression // suma "+"
    {
        public SumExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol)
        {
            validTypes[(WalleType.Sequence, WalleType.Sequence)] = WalleType.Sequence;

            validTypes[(WalleType.Measure, WalleType.Measure)] = WalleType.Measure;
        }


        public override object Evaluate()
        {
            if (left.ReturnType == WalleType.Measure)
                return (Medidas)left.Evaluate() + (Medidas)right.Evaluate();

            // concatenar secuencias
            return (double)left.Evaluate() + (double)right.Evaluate();
        }
    }

    public sealed class RestExpression : ArithmeticExpression // resta "-"
    {
        public RestExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol)
        {

            validTypes[(WalleType.Measure, WalleType.Measure)] = WalleType.Measure;
        }


        public override object Evaluate()
        {
            if (left.ReturnType == WalleType.Measure)
                return (Medidas)left.Evaluate() - (Medidas)right.Evaluate();

            return (double)left.Evaluate() - (double)right.Evaluate();
        }
    }

    public sealed class MultiplicativeExpression : ArithmeticExpression // multiplicación "*"
    {
        public MultiplicativeExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol)
        {

            validTypes[(WalleType.Number, WalleType.Measure)] = WalleType.Measure;

            validTypes[(WalleType.Measure, WalleType.Number)] = WalleType.Measure;
        }
        public override object Evaluate()
        {
            if (left.ReturnType == WalleType.Measure)
            {
                ReturnType = WalleType.Measure;
                return (Medidas)left.Evaluate() * (double)right.Evaluate();
            }
            return (double)left.Evaluate() * (double)right.Evaluate();
        }
    }

    public sealed class DivisionExpression : ArithmeticExpression // división "/"
    {
        public DivisionExpression(IExpression left, IExpression right, string operatorSymbol) : base(left, right, operatorSymbol)
        {

            validTypes[(WalleType.Measure, WalleType.Measure)] = WalleType.Number;
        }

        public override object Evaluate()
        {
            try
            {
                if (left.ReturnType == WalleType.Measure)
                    return (Medidas)left.Evaluate() / (Medidas)right.Evaluate();
                return (double)left.Evaluate() / (double)right.Evaluate();
            }
            catch (DivideByZeroException)
            {
                throw new DivideByZeroException($"{this}");
            }
        }
    }
    #endregion

    #region Undefined
    public sealed class UndefinedExpression : LiteralExpression // representar una expresión indefinida
    {
        public override WalleType ReturnType => WalleType.Undefined;

        public UndefinedExpression() : base(0) { }

        public override bool ConvertToBool() => false; // convertir a booleano
        public override string ToString() => "undefined"; // convertir a string
    }
    #endregion

    #region Secuence Literal
    public sealed class SequenceExpression : IExpression // representar una secuencia
    {
        public SequenceExpression(Secuencias data)
        {
            sequence = data;
        }
        private Secuencias sequence;
        public WalleType ReturnType => WalleType.Sequence;

        public void GetScope(Scope actual)
        {
            if (!sequence.isInRange)
            {
                foreach (var item in sequence)
                {
                    item.GetScope(actual);
                }
            }
        }
        public WalleType CheckSemantics() // chequear la semantica de la secuencia
        {
            if (!sequence.isInRange)
            {
                foreach (var item in sequence)
                {
                    item.CheckSemantics();
                }
            }
            return WalleType.Sequence;
        }
        public object Evaluate()
        {
            return sequence;
        }

        public bool ConvertToBool() // convertir a booleano
        {
            return sequence.Count() != 0;
        }
        public override string ToString() // convertir a string
        {
            return sequence.ToString();
        }
    }
    #endregion

    #region String Literal
    public sealed class TextLiteralExpression : LiteralExpression // representar un string
    {
        public override WalleType ReturnType => WalleType.Text;
        public TextLiteralExpression(object value) : base(value) { }

        public override string ToString() => $"\"{Value}\"";
    }
    #endregion

    #region Number Literal
    public sealed class NumberLiteralExpression : LiteralExpression // representar un número
    {
        public override WalleType ReturnType => WalleType.Number;
        public NumberLiteralExpression(object value) : base(value) { }
        public override bool ConvertToBool() => (double)Evaluate() != 0;
        public override string ToString() => $"{Value}";
    }
    #endregion

    #region Atomics
    public abstract class LiteralExpression : IExpression // representar expresiones literales
    {
        protected object Value { get; }
        public abstract WalleType ReturnType { get; }
        public LiteralExpression(object value)
        {
            Value = value;
        }
        public void GetScope(Scope Actual) { }
        public WalleType CheckSemantics()
        {
            return ReturnType;
        }
        public virtual object Evaluate() => Value; 
        public virtual bool ConvertToBool() => true;


    }
    #endregion
}

