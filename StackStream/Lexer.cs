using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream
{
    public class LexerException : Exception
    {
        public LexerException(string message)
            : base(message)
        { }
    }

    public class Lexer
    {
        public string Data
        {
            get;
            set;
        }

        public int Pointer
        {
            get;
            set;
        }

        private Lexer(string value)
        {
            Data = value;
            Pointer = 0;
        }

        public void SkipWhitespace()
        {
            while (Pointer < Data.Length && char.IsWhiteSpace(Data, Pointer))
                Pointer++;
        }

        public string Parse()
        {
            SkipWhitespace();
            if (Pointer >= Data.Length)
                return null;

            if (Data[Pointer] == '\'')
                return TakeUntilWhitespace();
            else if (Data[Pointer] == '`')
            {
                Pointer += 2;
                return Data.Substring(Pointer - 2, 2);
            }
            else if (Data[Pointer] == '{' || Data[Pointer] == '}' || char.IsSymbol(Data, Pointer))
                return Data.Substring(Pointer++, 1);

            return TakeUntilWhitespace();
        }

        public string TakeUntilWhitespace()
        {
            int start = Pointer;
            while (Pointer < Data.Length && !char.IsWhiteSpace(Data, Pointer) && Data[Pointer] != '}' && Data[Pointer] != '{')
                Pointer++;
            return Data.Substring(start, Pointer - start);
        }

        public bool HaveTokens
        {
            get
            {
                SkipWhitespace();
                return Pointer < Data.Length;
            }
        }

        public static Tokens.CodeBlock Parse(string value)
        {
            var lexer = new Lexer(value);
            return _Parse(lexer);
        }

        private static Tokens.CodeBlock _Parse(Lexer lexer)
        {
            var tokens = new List<IToken>();
            while (lexer.HaveTokens)
            {
                var token = lexer.Parse();
                if (token == null || token.Length == 0)
                    throw new LexerException("Failed to read token");
                int result;

                if (token[0] == '\'')
                    tokens.Add(new Tokens.Symbol(token.Substring(1)));
                else if (token[0] == '`')
                    tokens.Add(new Tokens.Number(token[1]));
                else if (int.TryParse(token, out result))
                    tokens.Add(new Tokens.Number(result));
                else if (token == "}")
                    break;
                else if (token == "{")
                    tokens.Add(_Parse(lexer));
                else
                    tokens.Add(new Tokens.Method(token));
            }

            return new Tokens.CodeBlock(tokens);
        }
    }
}
