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
            var token = exec.DataStack.Peek<IToken>();
            if (token is Tokens.PackedBlock)
            {
                var tokens = exec.DataStack.Pop<Tokens.PackedBlock>().Value;
                exec.DataStack.Push(new Tokens.CodeBlock(tokens));
            }
            else
            {
                var tokens = exec.DataStack.Pop<Tokens.Symbol>().Value;
                var method = exec.Methods[tokens];
                var value = ((Executor.CodeblockFunction) method).Tokens;
                exec.DataStack.Push(new Tokens.CodeBlock(value));
            }
        }

        [Function("from-codeblock")]
        public static void FromCodeblock(Executor exec)
        {
            var token = exec.DataStack.Peek<IToken>();
            if (token is Tokens.CodeBlock)
            {
                var tokens = exec.DataStack.Pop<Tokens.CodeBlock>().Value;
                exec.DataStack.Push(new Tokens.PackedBlock(tokens));
            }
            else
            {
                var tokens = exec.DataStack.Pop<Tokens.Symbol>().Value;
                var method = exec.Methods[tokens];
                var value = ((Executor.CodeblockFunction)method).Tokens;
                exec.DataStack.Push(new Tokens.PackedBlock(value));
            }
        }
    }
}
