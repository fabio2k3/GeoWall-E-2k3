﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoWallE_Fabio_2k3
{
    public class Lexer
    {
        private readonly string codeline;
        private int _position;
        private char Current => Peek(0);
        private char Peek(int distance)
        { 

            int index = _position + distance;

            if (index >= codeline.Length)
                return '\0';

            return codeline[index];
        }

        public Lexer(string codeline)
        {
            this.codeline = codeline;
        }

        public List<Token> GetTokenList() // crear lista de Tokens
        {
            Token currentToken;
            List<Token> output = new List<Token>();
            do
            {
                currentToken = GetToken();
                if (currentToken.Type != TokenType.WhiteSpaceToken && currentToken.Type != TokenType.CommentaryToken)
                    output.Add(currentToken);
            }
            while (currentToken.Type != TokenType.EndOfFileToken);

            return output;
        }

        private Token GetToken() // Obtener Tokens
        {
            TokenType kind;
            string text;

            if (char.IsDigit(Current))
                CompleteNumberLiteral(out kind, out text);

            else if (Current == '"')
                CompleteStringLiteral(out kind, out text);

            else if (char.IsWhiteSpace(Current))
                CompleteWhiteSpace(out kind, out text);

            else if (char.IsLetter(Current) || Current == '_')
                CompleteKeyWordOrIdentifier(out kind, out text);

            else if (Current == '/' && Peek(1) == '/')
                CompleteCommentaryToken(out kind, out text);

            else
                CompleteSymbol(out kind, out text);

            return new Token(kind, text);
        }

        // recorrer línea de codigo y encontrar numeros
        private void CompleteNumberLiteral(out TokenType kind, out string text)
        {
            kind = TokenType.NumberLiteralToken;

            int start = _position;
            bool IsAlreadyDecimal = false;

            while (char.IsDigit(Current) || (Current == '.' && !IsAlreadyDecimal && char.IsDigit(Peek(1))))
            {
                if (Current == '.')
                    IsAlreadyDecimal = true;

                _position++;
            }

            int length = _position - start;
            text = codeline.Substring(start, length);
            if (!double.TryParse(text, out _))
                throw new InvalidCastException($"\"{text}\" cannot be represented as number.");
        }

        // reocrrer linea de codigo y encontrar cadenas de texto
        private void CompleteStringLiteral(out TokenType kind, out string text)
        {
            _position++;    

            kind = TokenType.StringLiteralToken;

            var sb = new StringBuilder();
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                        throw new InvalidDataException($"Unterminated string literal : \"{sb}...");
                    case '"':
                        done = true;
                        _position++;
                        break;

                    default:
                        sb.Append(Current);
                        _position++;
                        break;
                }
            }
            text = sb.ToString();
        }

        // recorrer cada línea de código y encuentrar un espacio en blanco, determinar su posición de inicio y su longitud
        private void CompleteWhiteSpace(out TokenType kind, out string text)
        {
            kind = TokenType.WhiteSpaceToken;
            int start = _position;

            while (char.IsWhiteSpace(Current))
            {
                _position++;
            }

            int length = _position - start;
            text = codeline.Substring(start, length);
        }

        // encontrar palabras claves
        private void CompleteKeyWordOrIdentifier(out TokenType kind, out string text)
        {
            int start = _position;

            while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                _position++;
            }

            int length = _position - start;
            text = codeline.Substring(start, length);

            kind = CompilatorTools.GetKeyWordKind(text);
        }

        // encontrar simbolos
        private void CompleteSymbol(out TokenType kind, out string text)
        {
            if (CompilatorTools.OperatorsPool.TryGetValue(Current.ToString() + Peek(1).ToString() + Peek(2).ToString(), out kind))
            {
                text = Current.ToString() + Peek(1).ToString() + Peek(2).ToString();
            }

            else if (CompilatorTools.OperatorsPool.TryGetValue(Current.ToString() + Peek(1).ToString(), out kind))
            {
                text = Current.ToString() + Peek(1).ToString();
            }

            else if (CompilatorTools.OperatorsPool.TryGetValue(Current.ToString(), out kind))
            {
                text = Current.ToString();
            }
            else
            {
                System.Console.WriteLine(Current.ToString() + Peek(1).ToString() + Peek(2).ToString());
                kind = TokenType.BadToken;
                text = Current.ToString();
                throw new InvalidDataException($"Invalid Character : {text}");
            }
            _position += text.Length;
        }

        private void CompleteCommentaryToken(out TokenType kind, out string text)
        {
            kind = TokenType.CommentaryToken;
            _position += 2;    
            int start = _position;

            while (Current != '\n' && Current != '\0')
            {
                _position++;
            }

            int length = _position - start;
            text = codeline.Substring(start, length);

        }
    }
}
