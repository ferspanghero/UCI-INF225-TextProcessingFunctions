using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextProcessingFunctions.Core.Properties;

namespace TextProcessingFunctions.Core.TextProcesser
{   
    /// <summary>
    /// Represents a text processer that takes text files as input
    /// </summary>
    public class TextFileProcesser : ITextProcesser
    {
        #region Constructors
        public TextFileProcesser(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !Regex.IsMatch(filePath, "^*.txt$"))
                throw new ArgumentException(Resources.ArgumentException_InvalidTextFilePathMessage);

            _filePath = filePath;
            _indistinctTokenList = new List<Token>();
        }
        #endregion        

        #region Properties
        private string _filePath;
        private List<Token> _indistinctTokenList;
        #endregion

        #region Methods
        private void _AddToken(ICollection<Token> tokens, int bufferSize, ref char[] wordBuffer, ref int wordBufferCount)
        {
            // Converts the buffer into a lowercase string
            string word = new string(wordBuffer, 0, wordBufferCount).ToLower();                        
            Token token = new Token(word);

            // If it tries to add a token that already exists, HashSet<T> automatically discards it
            tokens.Add(token);

            // Resets word buffer
            wordBuffer = new char[bufferSize];
            wordBufferCount = 0;
        }        
        #endregion

        #region ITokenizer Implementation
        public IEnumerable<Token> Tokenize()
        {
            if (!File.Exists(_filePath))
                throw new ArgumentException(Resources.ArgumentException_NonExistingTextFileMessage);

            // Clears up the indistinct tokens list to ensure idempotency
            _indistinctTokenList.Clear();

            using (StreamReader reader = File.OpenText(_filePath))
            {
                int bufferSize = 1024;
                int charactersReadCount = 0;
                int textFileStreamCurrentPosition = 0;
                char[] buffer = new char[bufferSize];

                // A word buffer is necessary instead of storing simple indexes because a word can start in one chunk and end in another
                char[] wordBuffer = new char[bufferSize];
                int wordBufferCount = 0;

                while (!reader.EndOfStream)
                {
                    charactersReadCount = reader.Read(buffer, textFileStreamCurrentPosition, bufferSize);

                    if (buffer != null && charactersReadCount > 0)
                    {
                        // Only considers the number of characters read since it can be smaller than the buffer itself
                        for (int i = 0; i < charactersReadCount; i++)
                        {
                            // Only considers alphanumerical characters as valid for words
                            if (char.IsLetterOrDigit(buffer[i]))
                            {
                                wordBuffer[wordBufferCount++] = buffer[i];

                                // If it reads the last character of the file and it is alphanumerical, then we have a word
                                if (i == charactersReadCount - 1 && reader.EndOfStream)
                                    _AddToken(_indistinctTokenList, bufferSize, ref wordBuffer, ref wordBufferCount);
                            }

                            // If it hits a non-alphanumerical character and the word buffer is not empty, then we have a word
                            else if (wordBufferCount > 0)
                                _AddToken(_indistinctTokenList, bufferSize, ref wordBuffer, ref wordBufferCount);
                        }
                    }
                }
            }

            // Discards tokens repetition by return a distinct collection
            return
                _indistinctTokenList.Distinct();
        }

        public IEnumerable<KeyValuePair<Token, int>> ComputeWordFrequencies()
        {
            // Considering tokens are always unique, a Dictionary<TKey, TValue> will always provide 
            // a O(1) time complexity if it is necessary to search for a particular token
            Dictionary<Token, int> wordFrequencies = new Dictionary<Token, int>();

            // If there are no processed tokens, try to tokenize the input text file
            if (_indistinctTokenList == null || _indistinctTokenList.Count == 0)
                Tokenize();

            foreach (var token in _indistinctTokenList)
            {
                // If it tries to add a token that already exists, we should increment the existing token frequency number instead
                if (wordFrequencies.ContainsKey(token))
                    wordFrequencies[token]++;
                else
                    wordFrequencies.Add(token, 1);
            }

            return
                wordFrequencies;
        }
        #endregion
    }
}
