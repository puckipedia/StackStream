using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Tokens
{
    public class Method : IToken
    {
        public string Value
        {
            get;
            private set;
        }

        public Method(string value)
        {
            Value = value;
        }

        public IToken Duplicate()
        {
            return new Method(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
