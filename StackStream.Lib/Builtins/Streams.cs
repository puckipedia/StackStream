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
        [Function("stdinout")]
        public static void Stdinout(Executor exec)
        {
            exec.DataStack.Push(new Stdinout());
        }

        [Function("read-stream")]
        public static void ReadStream(Executor exec)
        {
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            exec.DataStack.Push(new Tokens.Number(stream.Read()));
        }

        [Function("write-stream")]
        public static void WriteStream(Executor exec)
        {
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            stream.Write((byte) value);
        }

        [Function("tell-stream")]
        public static void TellStream(Executor exec)
        {
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            exec.DataStack.Push(new Tokens.Number(stream.Tell()));
        }

        [Function("eof-stream")]
        public static void EofStream(Executor exec)
        {
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            exec.DataStack.Push(new Tokens.Number(stream.IsEOF ? 1 : 0));
        }

        [Function("seek-stream")]
        public static void SeekStream(Executor exec)
        {
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            if (!stream.Seek((int) value))
                throw new Exception("seek-stream failed!");
        }

        [Function("new-buffer")]
        public static void NewBuffer(Executor exec)
        {
            exec.DataStack.Push(new StackStream.Streams.Buffer());
        }

        [Function("read-buffer")]
        public static void ReadBuffer(Executor exec)
        {
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            if (!stream.Seek((int) value))
                throw new Exception("seek-stream failed!");
            exec.DataStack.Push(new Tokens.Number(stream.Read()));
        }

        [Function("write-buffer")]
        public static void WriteBuffer(Executor exec)
        {
            var write = exec.DataStack.Pop<Tokens.Number>().Value;
            var value = exec.DataStack.Pop<Tokens.Number>().Value;
            var stream = exec.DataStack.Pop<Tokens.Stream>();
            if (!stream.Seek((int) value))
                throw new Exception("seek-stream failed!");
            stream.Write((byte) write);
        }
    }
}
