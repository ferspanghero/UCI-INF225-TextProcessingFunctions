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
        /// Converts a text input words into tokens
        /// </summary>
        IEnumerable<Token> Tokenize();

        /// <summary>
        /// Calculates a text input words frequencies
        /// </summary>
        IEnumerable<KeyValuePair<Token, int>> ComputeWordFrequencies();
    }
}
