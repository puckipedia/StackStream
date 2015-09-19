using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Lib.Builtins
{
    public static class FileTcpIo
    {
        private class TcpStream : Tokens.Stream
        {
            private Stream _stream;
            private TcpClient _client;

            public TcpStream(string host, short port)
            {
                _client = new TcpClient(host, port);
                _stream = _client.GetStream();
            }

            public override bool IsEOF
            {
                get
                {
                    return !_client.Connected;
                }
            }

            public override byte Read()
            {
                return (byte) _stream.ReadByte();
            }

            public override bool Seek(int location)
            {
                return false;
            }

            public override int Tell()
            {
                return (int) _stream.Position;
            }

            public override void Write(byte val)
            {
                _stream.WriteByte(val);
            }

            ~TcpStream()
            {
                if (_client.Connected)
                    _client.Close();
            }
        }


        private class FileStream : Tokens.Stream
        {
            private System.IO.FileStream _stream;

            public FileStream(System.IO.FileStream stream)
            {
                _stream = stream;
            }

            public override bool IsEOF
            {
                get
                {
                    return _stream.Position == _stream.Length;
                }
            }

            public override byte Read()
            {
                return (byte) _stream.ReadByte();
            }

            public override bool Seek(int location)
            {
                return _stream.Seek(location, SeekOrigin.Begin) == location;
            }

            public override int Tell()
            {
                return (int) _stream.Position;
            }

            public override void Write(byte val)
            {
                _stream.WriteByte(val);
            }
        }

        public static void NewTcpStream(Executor exec)
        {
            var port = (short)exec.DataStack.Pop<Tokens.Number>().Value;
            var host = exec.DataStack.Pop<Tokens.PackedBlock>().AsString();
            exec.DataStack.Push(new TcpStream(host, port));
        }

        public static void ReadFile(Executor exec)
        {
            var file = exec.DataStack.Pop<Tokens.PackedBlock>().AsString();
            exec.DataStack.Push(new FileStream(File.Open(file, FileMode.Open, FileAccess.Read)));
        }

        public static void WriteFile(Executor exec)
        {
            var file = exec.DataStack.Pop<Tokens.PackedBlock>().AsString();
            exec.DataStack.Push(new FileStream(File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)));
        }
    }
}
