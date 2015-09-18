using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackStream
{
    public interface IToken
    {
        IToken Duplicate();
    }
}
