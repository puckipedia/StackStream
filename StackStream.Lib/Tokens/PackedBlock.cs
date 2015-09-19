using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Tokens
{
    public class PackedBlock : IToken
    {
        public List<IToken> Value
        {
            get;
            private set;
        }

        public PackedBlock(IEnumerable<IToken> tokens)
        {
            Value = tokens.ToList();
        }

        public string AsString()
        {
            return new string(Value.Select(a => (char)((Number)a).Value).ToArray());
        }

        public IToken Duplicate()
        {
            return new PackedBlock(Value.Select(a => a.Duplicate()));
        }
    }
}
