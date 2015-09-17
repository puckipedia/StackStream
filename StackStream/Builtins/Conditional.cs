using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream.Builtins
{
    public static class Conditional
    {
        [Function("if")]
        public static void If(Executor exec)
        {
            var code = exec.DataStack.Pop<Tokens.CodeBlock>().Value;
            var condition = exec.DataStack.Pop<Tokens.Number>().Value != 0;
            if (condition)
                exec.CodeStack.PushRange(code);
        }

        [Function("elseif")]
        public static void ElseIf(Executor exec)
        {
            var ifcode = exec.DataStack.Pop<Tokens.CodeBlock>().Value;
            var elsecode = exec.DataStack.Pop<Tokens.CodeBlock>().Value;
            var condition = exec.DataStack.Pop<Tokens.Number>().Value != 0;
            if (condition)
                exec.CodeStack.PushRange(ifcode);
            else
                exec.CodeStack.PushRange(elsecode);
        }
    }
}
