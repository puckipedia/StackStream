using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Tokens
{
    public class Symbol : IToken
    {
        public string Value
        {
            get;
            private set;
        }

        public Symbol(string value)
        {
            Value = value;
        }

        public IToken Duplicate()
        {
            return new Symbol(Value);
        }

        public override string ToString()
        {
            return "'" + Value;
        }
    }
}
