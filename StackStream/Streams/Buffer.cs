using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Streams
{
    class Buffer : Tokens.Stream
    {
        private int _location = 0;
        private byte[] _buffer = new byte[0];

        public override bool IsEOF
        {
            get
            {
                return _location >= _buffer.Length;
            }
        }

        public override byte Read()
        {
            if (_location >= _buffer.Length)
                throw new Exception("Reading outside bounds!");
            return _buffer[_location++];
        }

        public override bool Seek(int location)
        {
            if (location < 0)
                return false;
            _location = location;
            return true;
        }

        public override int Tell()
        {
            return _location;
        }

        public override void Write(byte val)
        {
            if (_location >= _buffer.Length)
                Array.Resize(ref _buffer, _location + 1);
            _buffer[_location++] = val;
        }
    }
}
