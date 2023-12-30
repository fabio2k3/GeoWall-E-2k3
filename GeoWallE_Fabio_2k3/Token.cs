using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string text)
        {
            Type = type;
            Value = text;
        }

        public override string ToString()
        {
            string output = $"{Type}";

            if (!string.IsNullOrEmpty(Value))
                output += $", \"{Value}\"";

            return output;
        }
    }
}
