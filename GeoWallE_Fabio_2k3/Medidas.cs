using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    // clase encargada de realizar todas las medidas para poder operar con ellas
    public sealed class Medidas : IComparable<Medidas>
    {
        public double Value { get; private set; }
        public Medidas(double value)
        {
            Value = value;
        }

        public int CompareTo(Medidas other)
        {
            if (other == null)
                throw new ArgumentNullException("Object instance not set to any value ");
            return Value.CompareTo(other.Value);
        }

        public static Medidas operator +(Medidas a) => new Medidas(a.Value);
        public static Medidas operator -(Medidas a) => new Medidas(-a.Value);
        public static Medidas operator +(Medidas a, Medidas b) => new Medidas(a.Value + b.Value);
        public static Medidas operator -(Medidas a, Medidas b) => new Medidas(Math.Abs(a.Value - b.Value));

        public static Medidas operator *(Medidas a, double scalar) => new Medidas(a.Value * Math.Truncate(Math.Abs(scalar)));
        public static double operator /(Medidas a, Medidas b) => (b.Value != 0) ? a.Value / b.Value : throw new ArgumentException("Zero Division Error");

        public double ToDouble() => Value;
        public float ToFloat() => (float)Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
