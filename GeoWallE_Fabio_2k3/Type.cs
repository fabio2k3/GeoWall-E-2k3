using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public enum WalleType
    {
        Number, Text, Undefined, Sequence, Void, Line, Point, Segment, Ray, Arc, Circle, Measure
    }

    public enum FigureType
    {
        Arc, Circle, Line, Point, Ray, Segment,
    }
    public enum TokenType
    {
        // Tokens
        BadToken, EndOfFileToken, WhiteSpaceToken, NumberLiteralToken, IdentifierToken, StringLiteralToken, CommentaryToken,

        // Palabras claves (Keywords)
        AndKwToken, ArcKwToken, CircleKwToken, ColorKwToken, DrawKwToken, ElseKwToken, IfKwToken, InKwToken, ImportKwToken,
        LetKwToken, LineKwToken, NotKwToken, OrKwToken, PointKwToken, RayKwToken, RestoreKwToken,
        SegmentKwToken, SequenceKwToken, ThenKwToken, UndefinedKwToken, SystemFunctionKwToken,

        // Colores
        ColorValueToken,

        // Delimitadores
        OpenParenthesisToken, CloseParenthesisToken, OpenKeyToken, CloseKeyToken, CommaSeparatorToken, SemiColonToken,

        // Operadores
        TripleDotToken,

        // Operadores Logicos
        EqualsToken, EqualsEqualsToken, NotEqualsToken, LessToken, LessEqualsToken, GreaterToken, GreaterEqualsToken,

        // Operadores Aritmeticos
        PlusSignToken, MinusSignToken, StarToken, SlashToken, PercentageSignToken, ExponentToken,
    }
}
