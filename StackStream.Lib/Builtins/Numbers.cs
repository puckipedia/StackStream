using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Builtins
{
    public static class Numbers
    {
        [Function("+")]
        public static void Add(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a + b));
        }

        [Function("*")]
        public static void Multiply(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a * b));
        }

        [Function("-")]
        public static void Subtract(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a - b));
        }

        [Function("/")]
        public static void Divide(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a / b));
        }

        [Function("|")]
        public static void BitwiseOr(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a | b));
        }

        [Function("&")]
        public static void BitwiseAnd(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a & b));
        }

        [Function("%")]
        public static void Mod(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a % b));
        }

        [Function("<")]
        public static void LessThan(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a < b ? 1 : 0));
        }

        [Function(">")]
        public static void MoreThan(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a > b ? 1 : 0));
        }

        [Function("<=")]
        public static void LessThanEquals(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a <= b ? 1 : 0));
        }

        [Function(">=")]
        public static void MoreThanEquals(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a >= b ? 1 : 0));
        }

        [Function("not")]
        public static void BitwiseNot(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(~b));
        }

        [Function("=")]
        public static void Equals(Executor exec)
        {
            var b = exec.DataStack.Pop<Tokens.Number>().Value;
            var a = exec.DataStack.Pop<Tokens.Number>().Value;
            exec.DataStack.Push(new Tokens.Number(a == b ? 1 : 0));
        }
    }
}
