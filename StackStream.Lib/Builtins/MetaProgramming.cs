using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Lib.Builtins
{
    public static class MetaProgramming
    {
        [Function("parse")]
        public static void Parse(Executor exec)
        {
            var str = exec.DataStack.Pop<Tokens.PackedBlock>().AsString();
            var result = Lexer.Parse(str).Value.First();
            exec.DataStack.Push(result);
        }

        [Function("to-codeblock")]
        public static void ToCodeblock(Executor exec)
        {
            var tokens = exec.DataStack.Pop<Tokens.PackedBlock>().Value;
            exec.DataStack.Push(new Tokens.CodeBlock(tokens));
        }

        [Function("from-codeblock")]
        public static void FromCodeblock(Executor exec)
        {
            var tokens = exec.DataStack.Pop<Tokens.CodeBlock>().Value;
            exec.DataStack.Push(new Tokens.PackedBlock(tokens));
        }
    }
}
