using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream
{
    public class TokenStack
    {
        private List<IToken> _tokens = new List<IToken>();
        public int Dive
        {
            get;
            set;
        }

        public void Push(IToken token)
        {
            _tokens.Insert(Dive, token);
        }

        public void PushRange(IEnumerable<IToken> tokens)
        {
            _tokens.InsertRange(Dive, tokens);
        }

        public int Count
        {
            get
            {
                return _tokens.Count - Dive;
            }
        }

        public T Pop<T>()
            where T : IToken
        {
            var token = (T) _tokens[Dive];
            _tokens.RemoveAt(Dive);
            return token;
        }

        public T Dig<T>(int index)
            where T : IToken
        {
            var token = (T)_tokens[Dive + index];
            _tokens.RemoveAt(Dive + index);
            return token;
        }

        public void Bury(IToken token, int index)
        {
            _tokens.Insert(index, token);
        }

        public override string ToString()
        {
            return
                string.Join(" ", _tokens.Skip(Dive).Reverse()) +
                " <- " +
                string.Join(" ", _tokens.Take(Dive).Reverse());
        }
    }
}
