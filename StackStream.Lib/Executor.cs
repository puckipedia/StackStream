﻿using System;
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
        private static Dictionary<string, Method> _GlobalMethods;

        private static List<string> _InternalCoded = new List<string>()
        {
            "{ dup 1 dive swap { drop } { while } elseif } 'while def",
            "{ swap dup 2 dig = } 'compare def",
            "{ dup 2 + dig dup 2 dig 1 + bury } 'dig' def",
            "{ stack-count swap 1 dive stack-count = assert } 'stack-check def",
            "{ 1 - swap dup 2 dive swap dup { drop drop } { repeat } elseif } 'repeat def",
        };

        public Dictionary<string, Method> Methods
        {
            get;
            private set;
        }
        static Executor() {
            var methods = typeof(Executor).Assembly.GetTypes().SelectMany(a =>
                a.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                 .Where(b => b.GetCustomAttributes(typeof(FunctionAttribute), false).Length == 1));

            _GlobalMethods = methods.ToDictionary(a => ((FunctionAttribute)a.GetCustomAttributes(typeof(FunctionAttribute), false).First()).Name, a => (Method)a.CreateDelegate(typeof(Method)));
        }

        public Executor()
        {
            CodeStack = new TokenStack();
            DataStack = new TokenStack();
            Methods = new Dictionary<string, Method>(_GlobalMethods);
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
                    Methods[method.Value](this);
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