using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            while (Pointer < Data.Length && (char.IsWhiteSpace(Data, Pointer) || Data[Pointer] == '#'))
            {
                if (Data[Pointer] == '#')
                {
                    int index = Data.IndexOf('\n', Pointer);
                    if (index < 0)
                        Pointer = Data.Length;
                    else
                        Pointer = index + 1;
                }
                Pointer++;
            }
        }

        private string ParseString()
        {
            bool escape = false;
            StringBuilder builder = new StringBuilder();

            while (Pointer < Data.Length && Data[Pointer] != '"')
                if (Data[Pointer] == '\\')
                {
                    escape = true;
                    Pointer++;
                }
                else if (escape)
                    switch (Data[Pointer])
                    {
                        case 'n':
                            builder.Append('\n');
                            Pointer++;
                            break;
                        case 'r':
                            builder.Append('\r');
                            Pointer++;
                            break;
                        case 'x':
                            if (Pointer + 2 < Data.Length)
                            {
                                throw new Exception("Not enough data to parse \\x!");
                            }
                            else
                            {
                                int value = Convert.ToInt32(Data.Substring(Pointer + 1, 2), 16);
                                builder.Append((char)value);
                                Pointer += 3;
                            }
                            break;
                        default:
                            builder.Append(Data[Pointer++]);
                            break;
                    }
                else
                    builder.Append(Data[Pointer++]);
            Pointer++;

            return builder.ToString();
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
            else if (Data[Pointer] == '"')
            {
                Pointer++;
                return "\"" + ParseString();
            }
            else if (Data[Pointer] == '{' || Data[Pointer] == '}' || char.IsSymbol(Data, Pointer))
                return Data.Substring(Pointer++, 1);

            return TakeUntilWhitespace();
        }

        public string TakeUntilWhitespace()
        {
            int start = Pointer;
            while (Pointer < Data.Length && !char.IsWhiteSpace(Data, Pointer) && Data[Pointer] != '}' && Data[Pointer] != '{' && Data[Pointer] != '#')
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
                BigInteger result;

                if (token[0] == '\'')
                    tokens.Add(new Tokens.Symbol(token.Substring(1)));
                else if (token[0] == '`')
                    tokens.Add(new Tokens.Number(token[1]));
                else if (BigInteger.TryParse(token, out result))
                    tokens.Add(new Tokens.Number(result));
                else if (token == "}")
                    break;
                else if (token == "{")
                    tokens.Add(_Parse(lexer));
                else if (token[0] == '"')
                {
                    tokens.Add(new Tokens.PackedBlock(token.Skip(1).Select(a => new Tokens.Number(a))));
                }
                else
                    tokens.Add(new Tokens.Method(token));
            }

            return new Tokens.CodeBlock(tokens);
        }
    }
}
