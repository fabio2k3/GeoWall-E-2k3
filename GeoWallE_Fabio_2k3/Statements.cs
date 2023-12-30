using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public interface IStatement : INode
    {
        void Execute();
    }
}
