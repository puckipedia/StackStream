using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Tokens
{
    public class CodeBlock : IToken
    {
        public IEnumerable<IToken> Value
        {
            get;
            private set;
        }

        public CodeBlock(IEnumerable<IToken> code)
        {
            Value = code;
        }

        public IToken Duplicate()
        {
            return new CodeBlock(Value);
        }

        public override string ToString()
        {
            return "{ " + string.Join(" ", Value) + " }";
        }
    }
}
