﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Builtins
{
    public static class Runtime
    {
        [Function("exec")]
        public static void Exec(Executor exec)
        {
            exec.CodeStack.PushRange(exec.DataStack.Pop<Tokens.CodeBlock>().Value);
        }

        [Function("def")]
        public static void Def(Executor exec)
        {
            var name = exec.DataStack.Pop<Tokens.Symbol>().Value;
            var code = exec.DataStack.Pop<Tokens.CodeBlock>().Value;

            exec.Methods[name] = new Executor.CodeblockFunction(code.ToList());
        }

        [Function("redef")]
        public static void Redef(Executor exec)
        {
            var to = exec.DataStack.Pop<Tokens.Symbol>().Value;
            var from = exec.DataStack.Pop<Tokens.Symbol>().Value;

            exec.Methods[to] = exec.Methods[from];
        }

        [Function("debug")]
        public static void Debug(Executor exec)
        {
            System.Diagnostics.Debug.WriteLine("Data: {0}\nCode: {1}", exec.DataStack.ToString(), exec.CodeStack.ToString());
        }

        [Function("assert")]
        public static void Assert(Executor exec)
        {
            var condition = exec.DataStack.Pop<Tokens.Number>().Value != 0;
            if (!condition)
                throw new Exception("Assert failed!");
        }
    }
}
