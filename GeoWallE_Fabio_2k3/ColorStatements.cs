using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    // restaurar color inicial
    public sealed class RestoreStatement : IStatement
    {
        public void GetScope(Scope Actual) { }
        public WalleType CheckSemantics() => WalleType.Void;
        public void Execute() => CompilatorTools.RestoreColor();
        public override string ToString() => "restore";
    }

    // agregar color para el trazado
    public sealed class ColorStatement : IStatement
    {
        public string Color { get; }
        public ColorStatement(string color)
        {
            Color = color;
        }

        public void GetScope(Scope Actual) { }
        public WalleType CheckSemantics() => WalleType.Void;

        public void Execute() => CompilatorTools.AddColor(Color);
        public override string ToString() => "color " + Color.ToString();
    }
}
