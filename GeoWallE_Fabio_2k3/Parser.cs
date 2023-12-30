using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public sealed class Parser
    {
        private List<Token> tokens;
        private int position; // puntero para ir moviendose entre los tokens
        private Token Current => Peek(0);

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Token Peek(int d)
        {
            // devuelve el token que se encuentra a una distancia d del actual
            int index = Math.Min(position + d, tokens.Count - 1);
            return tokens.ElementAt(index);
        }
        private Token NextToken()
        {
            // devuelve el token actual y cambia el puntero de posicion hacia el siguiente
            var current = Current;
            position++;
            return current;
        }

        private Token MatchKind(TokenType kind)
        {
            /* Chequea que el token actual concuerde con el tipo esperado, en caso contrario genera un error
            al comprobar siempre se cambia de token para no tener que hacerlo manualmente en cada momento   */

            if (Current.Type == kind)
                return NextToken();

            else
            {
                if (kind == TokenType.SemiColonToken)
                    throw new Exception("!Syntactic Error : \";\" expected");
                else
                    throw new Exception($"!Syntactic Error : received {Current.Type} while expecting {kind}");
            }
        }

        public SyntaxTree ParseCode() // crear arból 
        {
            return new SyntaxTree(ParseStatementList());
        }
        public List<IStatement> ParseStatementList(TokenType stopper = TokenType.EndOfFileToken)
        {
            List<IStatement> output = new List<IStatement>();
            while (Current.Type != stopper)
            {
                var statement = ParseStatement(); // analizar la declaración
                MatchKind(TokenType.SemiColonToken); // asegurar que el siguiente token sea un punto y coma
                output.Add(statement); // agregar a la lista 
            }
            return output;
        }

        // analizar cada Token y según su tipo realizar llevar a cabo método u otro
        private IStatement ParseStatement()
        {
            switch (Current.Type)
            {
                case TokenType.IdentifierToken:
                    if (Peek(1).Type == TokenType.OpenParenthesisToken)
                        return ParseFunction();
                    else
                        return ParseAssigment();

                case TokenType.ColorKwToken:
                    NextToken();
                    return new ColorStatement(MatchKind(TokenType.ColorValueToken).Value);

                case TokenType.RestoreKwToken:
                    NextToken();
                    return new RestoreStatement();

                case TokenType.IfKwToken:
                    return ParseCondicionalStatement();

                //case TokenType.ImportKwToken:
                //    return ParseImportStatement();

                case TokenType.DrawKwToken:
                    NextToken();
                    return new Draw_Statement(ParseExpression());

                case TokenType.LetKwToken:
                    return ParseLetInStatement();

                case TokenType.ArcKwToken:
                case TokenType.CircleKwToken:
                case TokenType.LineKwToken:
                case TokenType.PointKwToken:
                case TokenType.RayKwToken:
                case TokenType.SegmentKwToken:

                    if (Peek(1).Type == TokenType.OpenParenthesisToken)
                        return ParsePredefinedFunctionCall();

                    return ParseFigureStatement();

                case TokenType.SystemFunctionKwToken:
                    return ParsePredefinedFunctionCall();

                default:
                    throw new InvalidOperationException($"!Parsing Error : Invalid token {Current} in program flow");
            }
        }


        // analizar la sintaxis let-in
        private IStatement ParseLetInStatement()
        {
            MatchKind(TokenType.LetKwToken); // chequear que el primer token sea "let"
            List<IStatement> statements = ParseStatementList(TokenType.InKwToken); // almacenar las declaraciones de variables dentro del "let-in"
            MatchKind(TokenType.InKwToken); // chequear que el último token sea "in"

            return new Let_In_Statement(statements, ParseExpression());
        }

        // parsear declaración o llamado de función
        private IStatement ParseFunction()
        {
            string name = NextToken().Value;
            NextToken();  // saltando el parentesis

            List<IExpression> expressions = (Current.Type == TokenType.CloseParenthesisToken) ? new List<IExpression>() : ParseExpressionList(); /* en el caso que el siguiente token sea ")" devuelve una lista vacía,
                                                                                                                                                  caso contrario se llama al método ParseExpressionL */

            MatchKind(TokenType.CloseParenthesisToken); // asegurarse que el siguiente token es ")"

            if (Current.Type == TokenType.EqualsToken) // verifica si el token actual es un token de igual lo cual significa que se está realizando una declaración de función
            {
                NextToken();
                return new Declared_Function_Expression(name, ConvertToVariables(expressions), ParseExpression());
            }
            return new Function_Call_Expression(name, expressions); // devolver el nombre de la función, las variables y la expresión principal.
        }


        // crear una lista con los nombres de las variables
        private static List<string> ConvertToVariables(List<IExpression> expressions)
        {
            List<string> output = new List<string>();
            foreach (var item in expressions)
            {
                if (item is VariableExpression variable) // verifica si es de tipo VariableExpression
                    output.Add(variable.Name);
                else
                    throw new ArgumentException($"!Syntactic Error : Unexpected expression as function declaration parameter => {item}");
            }
            return output; // retorna la lista con los nombres de las variables
        }

        // analiza y devuelve una asignación en función del token actual
        private IStatement ParseAssigment()
        {
            string identifier = NextToken().Value;

            if (Current.Type == TokenType.CommaSeparatorToken)
            {
                NextToken();
                List<string> variableNames = ParseIdList();  // obtener una lista de nombres de variables
                variableNames.Insert(0, identifier); // inserta el identificador en la primera posición de la lista de nombres de variables
                MatchKind(TokenType.EqualsToken);
                return new MultiAssigmentStatement(variableNames, ParseExpression());
            }
            else if (Current.Type == TokenType.EqualsToken)
            {
                NextToken();
                return new SimpleAssigmentStatement(identifier, ParseExpression());
            }
            throw new InvalidOperationException($"!Parsing Error : Invalid token {Current} in program flow");
        }

        // analizar una secuencia de tokens en busca de identificadores y almacenarlos en una lista
        private List<string> ParseIdList()
        {
            List<string> output = new List<string>();
            while (true)
            {
                output.Add(MatchKind(TokenType.IdentifierToken).Value); // se espera recibir tokens de tipo "IdentifierToken"

                if (Current.Type != TokenType.CommaSeparatorToken) // en el caso que sea una coma se rompe el ciclo
                    break;
                else
                    NextToken();
            }
            return output;
        }
        private IStatement ParseCondicionalStatement()
        {
            NextToken(); // avanzar al siguiente token del código
            MatchKind(TokenType.OpenParenthesisToken); // verificar si el próximo token es un paréntesis de apertura
            IExpression condition = ParseExpression(); // analizar y obtener la condición de la declaración

            MatchKind(TokenType.CloseParenthesisToken); // comprobar si el siguiente token es un paréntesis de cierre
            MatchKind(TokenType.ThenKwToken); // verificar si el siguiente token es "then"

            IExpression trueBody = ParseExpression(); // analizar y obtener el cuerpo de código que se ejecutará si la condición es verdadera

            MatchKind(TokenType.ElseKwToken); // verificar si el siguiente token es "else"
            IExpression falseBody = ParseExpression(); // analizar y obtener el cuerpo de código que se ejecutará si la condición es falsa

            return new ConditionalExpression(condition, trueBody, falseBody);
        }

        // analizar y ejecutar una declaración de figura
        private IStatement ParseFigureStatement()
        {
            IExpression figureExpression;

            switch (NextToken().Type)
            {
                case TokenType.PointKwToken:
                    figureExpression = new PointExpression(NextToken().Value); // crea una nueva instancia de la clase PointExpression y se asigna el valor del siguiente token a figureExpression
                    break;

                case TokenType.CircleKwToken:
                    figureExpression = new CircleExpression(NextToken().Value); // crea una nueva instancia de la clase CircleExpression y asigna el valor del siguiente token a figureExpression
                    break;

                case TokenType.ArcKwToken:
                    throw new InvalidOperationException("!Semantic Error : Invalid Expression : \"arc\"");

                case TokenType.LineKwToken:
                case TokenType.RayKwToken:
                case TokenType.SegmentKwToken:
                    figureExpression = new LineExpression(NextToken().Value, Peek(-2).Type); // crea una nueva instancia de la clase LineExpression y asigna el valor del siguiente token y el tipo del token anterior a figureExpression
                    break;

                default:
                    throw new ArgumentException($"!Syntactic Error : Unexpected token {Peek(-1)}");
            }

            return new SimpleAssigmentStatement(Peek(-1).Value, figureExpression);
        }

        // se encarga de analizar una llamada a una función predefinida
        private IStatement ParsePredefinedFunctionCall()
        {
            string name = NextToken().Value; // obtener el nombre de la función
            MatchKind(TokenType.OpenParenthesisToken); // verificar si el siguiente token es un "("

            // en caso contrario se llama a la función ParseExpressionList para analizar la lista de expresiones pasadas como argumentos
            List<IExpression> expressions = (Current.Type == TokenType.CloseParenthesisToken) ? new List<IExpression>() : ParseExpressionList();

            MatchKind(TokenType.CloseParenthesisToken); // // verificar si el siguiente token es un ")"
            return new Function_Call_Expression(name, expressions);
        }

        // almacenar expresiones separadas por coma
        private List<IExpression> ParseExpressionList()
        {
            List<IExpression> output = new List<IExpression>();
            while (true)
            {
                output.Add(ParseExpression());
                if (Current.Type != TokenType.CommaSeparatorToken) // verificar si no es un token de separador ","
                    break;
                NextToken();
            }
            return output;
        }
        private IExpression ParseExpression()
        {
            return ParseLogicalOr();
        }

        // construir un árbol a partir de expresiones separadas por un "or"
        private IExpression ParseLogicalOr()
        {
            var left = ParseLogicalAnd();
            while (Current.Type == TokenType.OrKwToken)
            {
                var operatorToken = NextToken().Value;
                var right = ParseLogicalAnd();
                left = new OrExpression(left, right, operatorToken);
            }
            return left;
        }

        // construir un árbol a partir de expresiones separadas por un "and"
        private IExpression ParseLogicalAnd()
        {
            var left = ParseEqualityOperator();
            while (Current.Type == TokenType.AndKwToken)
            {
                var operatorToken = NextToken().Value;
                var right = ParseEqualityOperator();
                left = new AndExpression(left, right, operatorToken);
            }
            return left;
        }

        // analizar expresiones separadas por "==" o "!="
        private IExpression ParseEqualityOperator()
        {
            var left = ParseComparison();
            while (Current.Type == TokenType.EqualsEqualsToken || Current.Type == TokenType.NotEqualsToken)
            {
                var operatorToken = NextToken();
                var right = ParseComparison(); // analizar la parte derecha de la expresión de igualdad

                // comparar si son iguales
                if (operatorToken.Type == TokenType.EqualsEqualsToken)
                    left = new EqualityExpression(left, right, operatorToken.Value);
                else
                    left = new NotEqualityExpression(left, right, operatorToken.Value);
            }
            return left;
        }

        // realizar comparación de expresiones matématicas
        private IExpression ParseComparison()
        {
            var left = ParseSumOrRest();
            while (IsComparerOperator(Current))
            {
                var operatorToken = NextToken();
                var right = ParseSumOrRest();

                // dependiendo del tipo de operador realiza una función u otra
                switch (operatorToken.Type)
                {
                    case TokenType.GreaterToken: // ">"
                        left = new GreaterExpression(left, right, operatorToken.Value);
                        break;

                    case TokenType.GreaterEqualsToken: // ">="
                        left = new GreaterEqualsExpression(left, right, operatorToken.Value);
                        break;

                    case TokenType.LessToken: // "<"
                        left = new LessExpression(left, right, operatorToken.Value);
                        break;

                    case TokenType.LessEqualsToken: // "<0"
                        left = new LessEqualsExpression(left, right, operatorToken.Value);
                        break;
                }
            }
            return left;
        }
        
        // verificar si el token es de tipo "Comparador"
        private bool IsComparerOperator(Token current)
        {
            TokenType[] logicOperators = new TokenType[]
            {
            TokenType.GreaterToken,TokenType.GreaterEqualsToken , TokenType.LessToken , TokenType.LessEqualsToken
            };

            foreach (var item in logicOperators)
            {
                if (current.Type == item) { return true; } // comparar token actual con el tipo de token del objeto pasado 
            }
            return false;
        }

        //  analizar expresiones de suma o resta
        private IExpression ParseSumOrRest()
        {
            var left = ParseMultiplicationOrDivision();
            while (Current.Type == TokenType.PlusSignToken || Current.Type == TokenType.MinusSignToken)
            {
                var operatorToken = NextToken();
                var right = ParseMultiplicationOrDivision();
                if (operatorToken.Type == TokenType.PlusSignToken)
                    left = new SumExpression(left, right, operatorToken.Value);
                else
                    left = new RestExpression(left, right, operatorToken.Value);
            }

            return left;
        }

        //  analizar expresiones de multiplicación o división
        private IExpression ParseMultiplicationOrDivision()
        {
            var left = ParseTerm();
            while (Current.Type == TokenType.StarToken || Current.Type == TokenType.SlashToken)
            {
                var operatorToken = NextToken();
                var right = ParseTerm();

                if (operatorToken.Type == TokenType.StarToken)
                    left = new MultiplicativeExpression(left, right, operatorToken.Value);
                else
                    left = new DivisionExpression(left, right, operatorToken.Value);
            }
            return left;
        }

        // analiza cada token y realiza una función en dependencia de este
        private IExpression ParseTerm()
        {
            switch (Current.Type)
            {
                case TokenType.NotKwToken:
                    return ParseNotOperator();

                case TokenType.MinusSignToken:
                    return ParseNegativeOperator();

                case TokenType.NumberLiteralToken:
                    return new NumberLiteralExpression(double.Parse(NextToken().Value));

                case TokenType.StringLiteralToken:
                    return new TextLiteralExpression(NextToken().Value);

                case TokenType.OpenParenthesisToken:
                    NextToken();
                    var expression = ParseExpression();
                    MatchKind(TokenType.CloseParenthesisToken);
                    return expression;

                case TokenType.OpenKeyToken:
                    return ParseSequence();

                case TokenType.UndefinedKwToken:
                    NextToken();
                    return new UndefinedExpression();

                case TokenType.IdentifierToken:
                    if (Peek(1).Type == TokenType.OpenParenthesisToken)
                        return (IExpression)ParseFunction();
                    else
                        return new VariableExpression(NextToken());

                case TokenType.IfKwToken:
                    return (IExpression)ParseCondicionalStatement();

                case TokenType.LetKwToken:
                    return (IExpression)ParseLetInStatement();

                case TokenType.DrawKwToken:
                    NextToken();
                    return new Draw_Statement(ParseExpression());

                case TokenType.SystemFunctionKwToken:
                case TokenType.ArcKwToken:
                case TokenType.CircleKwToken:
                case TokenType.LineKwToken:
                case TokenType.PointKwToken:
                case TokenType.RayKwToken:
                case TokenType.SegmentKwToken:

                    if (Peek(1).Type == TokenType.OpenParenthesisToken)
                        return (IExpression)ParsePredefinedFunctionCall();
                    else
                        throw new InvalidOperationException($"!Syntactic Error : Unexpected token in program flow {Current}");

                default:
                    throw new InvalidOperationException($"!Syntactic Error : Unexpected token in program flow {Current}");
            }
        }

        // analizar operador de negación en una expresión matemática
        private IExpression ParseNegativeOperator()
        {
            var operatorToken = NextToken();
            var Term = ParseTerm(); // analizar y obtener el término de la expresión matemática después del operador
            return new NegativeOperatorExpression(Term, operatorToken.Value);
        }

        // analizar y devolver una expresión que utiliza el operador de negación "not"
        private IExpression ParseNotOperator()
        {
            var operatorToken = NextToken();
            IExpression Term;

            if (Current.Type == TokenType.MinusSignToken)
                Term = ParseNegativeOperator(); // analizar y obtener una expresión negativa
            else
                Term = ParseTerm(); // analizar y obtener una expresión normal

            return new NotOperatorExpression(Term, operatorToken.Value);
        }

        // verificar es una secuencia dentro de un rango específico, una secuencia vacía o una secuencia regular
        private IExpression ParseSequence()
        {
            NextToken(); // obtiene el siguiente token del código fuente

            if (Peek(1).Type == TokenType.TripleDotToken)
                return ParseSequenceInRange(); // analizar y parsear la secuencia dentro de un rango específico

            else if (Current.Type == TokenType.CloseKeyToken)
            {
                NextToken();
                return new SequenceExpression(new Secuencias(Array.Empty<IExpression>())); 
            }
            return ParseRegularSequence(); // analizar y parsear una secuencia regular
        }

        // crear una secuencia de expresiones a partir de una lista de expresiones
        private IExpression ParseRegularSequence()
        {
            List<IExpression> items = ParseExpressionList(); // devuelve una lista de expresiones y guarda ese resultado en la variable
            MatchKind(TokenType.CloseKeyToken); // erificar si el tipo de token coincide con el tipo de token esperado
            var sequence = new Secuencias(items);
            return new SequenceExpression(sequence);
        }

        // analizar una secuencia de números en un rango específico
        private IExpression ParseSequenceInRange()
        {
            int min = int.Parse(MatchKind(TokenType.NumberLiteralToken).Value); // convierte en un entero mínimo
            NextToken(); 

            int max = (Current.Type == TokenType.CloseKeyToken) ? int.MaxValue : int.Parse(NextToken().Value); // revisa si el token actual es del tipo TokenType.CloseKeyToken

            // erifica que el siguiente token sea del tipo TokenType.CloseKeyToken y realiza una coincidencia
            MatchKind(TokenType.CloseKeyToken);

            var sequence = new Secuencias(min, max);
            return new SequenceExpression(sequence);
        }
    }
}