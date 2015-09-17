using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Streams
{
    public class Stdinout : Tokens.Stream
    {
        public override bool IsEOF
        {
            get
            {
                return false;
            }
        }

        public override byte Read()
        {
            return (byte) Console.Read();
        }

        public override bool Seek(int location)
        {
            return false;
        }

        public override int Tell()
        {
            return -1;
        }

        public override void Write(byte val)
        {
            Console.Write((char) val);
        }
    }
}
