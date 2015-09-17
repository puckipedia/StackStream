using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Tokens
{
    public abstract class Stream : IToken
    {
        public abstract byte Read();

        public abstract void Write(byte val);

        public abstract bool Seek(int location);

        public abstract int Tell();

        public abstract bool IsEOF
        {
            get;
        }

        public IToken Duplicate()
        {
            return this;
        }
    }
}
