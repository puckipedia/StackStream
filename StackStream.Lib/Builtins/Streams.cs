using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackStream.Streams;

namespace StackStream.Builtins
{
    public static class Streams
    {
        [Function("stream-stdio")]
        public static void Stdinout(Executor exec)
        {
            exec.DataStack.Push(new Stdinout());
        }

        [Function("stream-read")]
        public static void ReadStream(Executor exec)
        {
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            exec.DataStack.Push(new Tokens.Number(stream.Read()));
        }

        [Function("stream-write")]
        public static void WriteStream(Executor exec)
        {
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            stream.Write((byte) value);
        }

        [Function("stream-tell")]
        public static void TellStream(Executor exec)
        {
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            exec.DataStack.Push(new Tokens.Number(stream.Tell()));
        }

        [Function("stream-eof?")]
        public static void EofStream(Executor exec)
        {
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            exec.DataStack.Push(new Tokens.Number(stream.IsEOF ? 1 : 0));
        }

        [Function("stream-seek")]
        public static void SeekStream(Executor exec)
        {
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            if (!stream.Seek((int) value))
                throw new Exception("stream-seek failed!");
        }

        [Function("buffer-new")]
        public static void NewBuffer(Executor exec)
        {
            exec.DataStack.Push(new StackStream.Streams.Buffer());
        }

        [Function("buffer-read")]
        public static void ReadBuffer(Executor exec)
        {
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            if (!stream.Seek((int) value))
                throw new Exception("stream-seek failed!");
            exec.DataStack.Push(new Tokens.Number(stream.Read()));
        }

        [Function("buffer-write")]
        public static void WriteBuffer(Executor exec)
        {
            var write = exec.DataStack.Pop<Tokens.Number>().Value;
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            if (!stream.Seek((int) value))
                throw new Exception("stream-seek failed!");
            stream.Write((byte) write);
        }
    }
}
