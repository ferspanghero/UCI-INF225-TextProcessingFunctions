using System.Collections.Generic;

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
        /// Calculates a text input word frequencies
        /// </summary>
        IEnumerable<KeyValuePair<Token, int>> ComputeWordFrequencies();

        /// <summary>
        /// Calculates a text input 2-gram frequencies
        /// </summary>
        IEnumerable<KeyValuePair<TwoGram, int>> ComputeTwoGramFrequencies();

        /// <summary>
        /// Calculates a text input palindrome frequencies
        /// </summary>
        IEnumerable<KeyValuePair<string, int>> ComputePalindromeFrequencies();
    }
}
