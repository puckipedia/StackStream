using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Tokens
{
    public class Number : IToken
    {
        public int Value
        {
            get;
            private set;
        }

        public Number(int val)
        {
            Value = val;
        }

        public IToken Duplicate()
        {
            return new Number(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
