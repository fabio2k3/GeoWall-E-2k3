using Microsoft.VisualBasic.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public sealed class Secuencias : IEnumerable<IExpression>
    {
        public Secuencias(IEnumerable<IExpression> data)
        {
            this.data = data;
            isInRange = false;
        }
        public Secuencias(int min, int max)
        {
            this.min = min;
            this.max = max;
            isInRange = true;
            data = new List<IExpression>();
        }
        public bool isInRange { get; }
        private readonly int min;
        private readonly int max;
        public WalleType ItemsType // devolver el tipo de los elementos de la secuencia
        {
            get
            {
                if (isInRange) // si está presente el rango númerico
                    return WalleType.Number;

                return GetItemsType(); // caso contrario que no esté
            }
        }

        private WalleType GetItemsType() // obtener un enumerador de la secuencia y verifica si está vacía
        {
            var classEnumerator = GetEnumerator();

            if (!classEnumerator.MoveNext()) // caso que esté vacía
                return WalleType.Undefined;

            WalleType returnType = classEnumerator.Current.ReturnType;
            foreach (var item in data)
            {
                if (item.ReturnType != returnType && item.ReturnType != WalleType.Undefined) // verificar si el tipo de retorno de cada elemento es igual al tipo de retorno del primer elemento
                    throw new ArgumentException($"Sequence's elements must have the same return type : {item}");
            }
            return returnType;
        }

        private readonly IEnumerable<IExpression> data;

        public int Count() // cantidad de elementos de la secuencia
        {
            if (isInRange)
                return (max == int.MaxValue) ? -1 : max - min;
            return data.Count();
        }

        public IEnumerator<IExpression> GetEnumerator()
        {
            if (isInRange)
                return new InRangeEnumerator(min, max);
            return data.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Secuencias operator +(Secuencias a, Secuencias b)
        {
            return new Secuencias(Concatenate(a, b));
        }

        public static Secuencias operator +(Secuencias a, Undefined b)
        {
            return a;
        }
        public static IEnumerable<IExpression> Concatenate(Secuencias a, Secuencias b)
        {
            foreach (var itemA in a)
            {
                yield return itemA;
            }
            foreach (var itemB in b)
            {
                yield return itemB;
            }
        }

        // devuelve una nueva instancia de "Secuencias" que contiene los elementos de la secuencia original a partir de la posición especificada
        public Secuencias Rest(int position)
        {
            if (isInRange)
                return new Secuencias(position + 1, max);
            return new Secuencias(RestAsEnumerable(position));
        }

        // obtener una enumeración de los elementos de la secuencia a partir de la posición especificada
        private IEnumerable<IExpression> RestAsEnumerable(int position)
        {
            int i = 0;
            foreach (var item in data)
            {
                if (i > position)
                    yield return item;
                i++;
            }
        }
    }

    public class Undefined
    {

        public static Undefined operator +(Undefined a, object b)
        {
            return new Undefined();
        }
    }

    internal class InRangeEnumerator : IEnumerator<IExpression>
    {
        public InRangeEnumerator(int min, int max)
        {
            Min = min;
            Max = max;
            Reset();
        }
        public int Min { get; }
        public int Max { get; }
        private int num;
        private bool isCurrent;

        public IExpression Current
        {
            get
            {
                if (isCurrent) return new NumberLiteralExpression(num); // Si el número actual está dentro del rango
                throw new InvalidOperationException("SequenceInRange enumerator out of position");
            }

        }

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            num++;
            isCurrent = num < Max;
            return isCurrent;
        }

        public void Reset() // establece el número actual en el valor mínimo menos uno
        {
            num = Min - 1;
        }
    }
}
