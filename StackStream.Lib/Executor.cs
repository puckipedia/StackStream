using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream
{
    public class FunctionAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public FunctionAttribute(string name)
        {
            Name = name;
        }
    }

    public class Executor
    {
        public interface IFunction
        {
            void Run(Executor exec);
        }

        public class NativeFunction : IFunction
        {
            private Method _methodToRun;
            public NativeFunction(Method methodToRun)
            {
                _methodToRun = methodToRun;
            }

            public void Run(Executor exec)
            {
                _methodToRun(exec);
            }
        }

        public class CodeblockFunction : IFunction
        {
            public List<IToken> Tokens
            {
                get;
                private set;
            }

            public CodeblockFunction(List<IToken> tokens)
            {
                Tokens = tokens;
            }

            public void Run(Executor exec)
            {
                exec.CodeStack.PushRange(Tokens);
            }
        }

        public TokenStack CodeStack
        {
            get;
            private set;
        }

        public TokenStack DataStack
        {
            get;
            private set;
        }

        public delegate void Method(Executor exec);
        private static Dictionary<string, IFunction> _GlobalMethods;

        private static List<string> _InternalCoded = new List<string>()
        {
            "{ dup 1 dive swap { drop } { while } elseif } 'while def",
            "{ swap dup 2 dig = } 'compare def",
            "{ dup 1 + { dup } swap dive dig } 'dig' def",
            "{ dup { drop drop } { 1 - swap dup 2 dive swap repeat } elseif } 'repeat def",
            "{ count-stack swap 1 dive count-stack = assert } 'stack-check def",
            "{ { drop } swap repeat } 'dropn def",
            "{ swap { unpack } 1 dive swap { unpack } 1 dive + pack } 'concat-packed def",
            "{ { unpack } 2 dive swap dup 3 + { drop } swap dive 1 + bury pack } 'replace-packed def",
            "{ { unpack } 1 dive 1 + dig { 1 - dropn } 1 dive } 'dig'-packed def",
            "{ dup { { 'uninitialised-element } swap repeat } 1 dive pack } 'new-packed def",
            "{ { unpack } 1 dive dup { - pack } 1 dive swap { dropn } 1 dive } 'shrink-packed def",
        };

        public Dictionary<string, IFunction> Methods
        {
            get;
            private set;
        }
        static Executor() {
            var methods = typeof(Executor).Assembly.GetTypes().SelectMany(a =>
                a.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                 .Where(b => b.GetCustomAttributes(typeof(FunctionAttribute), false).Length == 1));

            _GlobalMethods = methods.ToDictionary(a => ((FunctionAttribute)a.GetCustomAttributes(typeof(FunctionAttribute), false).First()).Name, a => (IFunction) new NativeFunction((Method)a.CreateDelegate(typeof(Method))));
        }

        public Executor()
        {
            CodeStack = new TokenStack();
            DataStack = new TokenStack();
            Methods = new Dictionary<string, IFunction>(_GlobalMethods);
            foreach (var line in _InternalCoded.Reverse<string>())
                CodeStack.PushRange(Lexer.Parse(line).Value);

            while (CodeStack.Count > 0)
                Cycle();
        }

        public void Cycle()
        {
            var token = CodeStack.Pop<IToken>();

            if (token is Tokens.Method)
            {
                var method = (Tokens.Method) token;

                if (Methods.ContainsKey(method.Value))
                    Methods[method.Value].Run(this);
                else
                    throw new Exception($"Unknown method {method.Value}!");
            }
            else
            {
                DataStack.Push(token);
            }
        }
    }
}
