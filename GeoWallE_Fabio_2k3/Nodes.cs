using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GeoWallE_Fabio_2k3
{
    public interface INode
    {
        void GetScope(Scope Actual);
        WalleType CheckSemantics();
    }

    public sealed class SyntaxTree // representar el árbol
    {
        public List<IStatement> Program { get; private set; }
        public Scope Enviroment { get; private set; }

        public SyntaxTree(List<IStatement> program)
        {
            Program = program;
            Enviroment = new Scope();
        }
        private void GetScope()
        {
            foreach (var statement in Program)
            {
                statement.GetScope(Enviroment);
            }

        }
        private void CheckSemantics()
        {
            foreach (var statement in Program)
            {
                statement.CheckSemantics();
            }
        }

        private void Execute()
        {
            foreach (var statement in Program)
            {
                statement.Execute();
            }
        }

        public int Run()
        {
            GetScope();
            CheckSemantics();
            Execute();
            return 0;
        }
    }
}
