using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Lib.Builtins
{
    public class Packed
    {

        [Function("packed-make")]
        public static void Pack(Executor exec)
        {
            var count = exec.DataStack.Pop<Tokens.Number>().Value;
            List<IToken> Tokens = new List<IToken>();
            for (int i = 0; i < count; i++)
            {
                Tokens.Add(exec.DataStack.Pop<IToken>());
            }

            exec.DataStack.Push(new Tokens.PackedBlock(Tokens));
        }

        [Function("packed-unmake")]
        public static void Unpack(Executor exec)
        {
            var block = exec.DataStack.Pop<Tokens.PackedBlock>().Value;
            var count = block.Count;

            exec.DataStack.PushRange(block);
            exec.DataStack.Push(new Tokens.Number(count));
        }

        [Function("packed-size")]
        public static void PackSize(Executor exec)
        {
            var block = exec.DataStack.Pop<Tokens.PackedBlock>().Value;
            exec.DataStack.Push(new Tokens.Number(block.Count));
        }

        [Function("packed-reverse")]
        public static void ReversePacked(Executor exec)
        {
            var block = exec.DataStack.Pop<Tokens.PackedBlock>().Value;
            block.Reverse();
            exec.DataStack.Push(new Tokens.PackedBlock(block));
        }

        [Function("packed-concat")]
        public static void PackedConcat(Executor exec)
        {
            var first = exec.DataStack.Pop<Tokens.PackedBlock>().Value;
            exec.DataStack.Peek<Tokens.PackedBlock>().Value.AddRange(first);
        }

        [Function("packed-get")]
        public static void PackedGet(Executor exec)
        {
            var loc = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(exec.DataStack.Peek<Tokens.PackedBlock>().Value[(int) loc].Duplicate());
        }

        [Function("packed-set")]
        public static void PackedSet(Executor exec)
        {
            var value = exec.DataStack.Pop<IToken>();
            var loc = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Peek<Tokens.PackedBlock>().Value[(int)loc] = value.Duplicate();
        }

        [Function("packed-new")]
        public static void PackedNew(Executor exec)
        {
            exec.DataStack.Push(new Tokens.PackedBlock(Enumerable.Repeat<IToken>(null, (int)exec.DataStack.Pop<Tokens.Number>().Value)));
        }

        [Function("packed-resize")]
        public static void PackedResize(Executor exec)
        {
            var size = (int) exec.DataStack.Pop<Tokens.Number>().Value;
            var value = exec.DataStack.Peek<Tokens.PackedBlock>().Value;
            if (value.Count > size)
                value.RemoveRange(value.Count - size - 1, size);
            else if (value.Count < size)
                value.AddRange(Enumerable.Repeat<IToken>(null, size - value.Count));
        }

    }
}
