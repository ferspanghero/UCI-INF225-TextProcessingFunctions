using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessingFunctions.Core.TextProcesser
{   
    /// <summary>
    /// Defines a processor which takes text as input and performs a set of operations
    /// </summary>
    public interface ITextProcesser
    {
        /// <summary>
        /// Converts a text input into Tokens
        /// </summary>
        IEnumerable<Token> Tokenize();

        /// <summary>
        /// Calculates a text input words frequencies
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, int>> ComputeWordFrequencies();
    }
}
