using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessingFunctions.Core.Tokens
{   
    public interface ITokenizer
    {
        IEnumerable<Token> Tokenize();
    }
}
