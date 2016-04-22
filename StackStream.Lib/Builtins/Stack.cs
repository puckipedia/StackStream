using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Builtins
{
    public static class Stack
    {
        [Function("dup")]
        public static void Dup(Executor exec)
        {
            var token = exec.DataStack.Pop<IToken>();
            exec.DataStack.Push(token.Duplicate());
            exec.DataStack.Push(token);
        }

        [Function("drop")]
        public static void Drop(Executor exec)
        {
            exec.DataStack.Pop<IToken>();
        }

        [Function("dive")]
        public static void Dive(Executor exec)
        {
            var count = exec.DataStack.Pop<Tokens.Number>().Value;
            var code = exec.DataStack.Pop<Tokens.CodeBlock>().Value;

            exec.DataStack.Dive += (int) count;

            exec.CodeStack.PushRange(new IToken[] { new Tokens.Number(-count), new Tokens.Method(" dive") });
            exec.CodeStack.PushRange(code);
        }

        [Function(" dive")]
        public static void InternalDive(Executor exec)
        {
            var count = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Dive += (int) count;
        }

        [Function("swap")]
        public static void Swap(Executor exec)
        {
            var a = exec.DataStack.Pop<IToken>();
            var b = exec.DataStack.Pop<IToken>();
            exec.DataStack.Push(a);
            exec.DataStack.Push(b);
        }

        [Function("dig")]
        public static void Dig(Executor exec)
        {
            var count = exec.DataStack.Pop<Tokens.Number>().Value;
            var token = exec.DataStack.Dig<IToken>((int) count);

            exec.DataStack.Push(token);
        }

        [Function("bury")]
        public static void Bury(Executor exec)
        {
            var count = exec.DataStack.Pop<Tokens.Number>().Value;
            var token = exec.DataStack.Pop<IToken>();

            exec.DataStack.Bury(token, (int) count);
        }

        [Function("stack-size")]
        public static void StackCount(Executor exec)
        {
            exec.DataStack.Push(new Tokens.Number(exec.DataStack.Count));
        }
    }
}
