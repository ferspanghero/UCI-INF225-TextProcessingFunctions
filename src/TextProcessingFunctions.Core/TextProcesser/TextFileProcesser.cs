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

            this._filePath = filePath;
        }
        #endregion        

        #region Properties
        private string _filePath;
        #endregion

        #region Delegates
        private delegate void TextProcessingAction<TOutput>(TOutput output, int bufferSize, ref char[] wordBuffer, ref int wordBufferCount);
        #endregion

        #region Methods
        private void _AddToken(HashSet<Token> tokens, int bufferSize, ref char[] wordBuffer, ref int wordBufferCount)
        {
            string word = _ExtractWord(bufferSize, ref wordBuffer, ref wordBufferCount);

            Token token = new Token(word);

            // If it tries to add a token that already exists, HashSet<T> automatically discards it
            tokens.Add(token);
        }        

        private void _AddWordFrequency(Dictionary<string, int> tokens, int bufferSize, ref char[] wordBuffer, ref int wordBufferCount)
        {
            string word = _ExtractWord(bufferSize, ref wordBuffer, ref wordBufferCount);

            // If it tries to add a token that already exists, we should increment the existing token frequency number instead
            if (tokens.ContainsKey(word))
                tokens[word]++;
            else
                tokens.Add(word, 1);          
        }

        private string _ExtractWord(int bufferSize, ref char[] wordBuffer, ref int wordBufferCount)
        {
            // Converts the buffer into a lowercase string
            string word = new string(wordBuffer, 0, wordBufferCount).ToLower();

            // Resets word buffer
            wordBuffer = new char[bufferSize];
            wordBufferCount = 0;
            return word;
        }

        /// <summary>
        /// Auxiliar method that contains the main text processing algorithm
        /// </summary>
        private TOutput _ProcessText<TOutput>(TOutput output, TextProcessingAction<TOutput> textProcessingAction)
        {
            if (!File.Exists(_filePath))
                throw new ArgumentException(Resources.ArgumentException_NonExistingTextFileMessage);

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
                                    textProcessingAction(output, bufferSize, ref wordBuffer, ref wordBufferCount);
                            }

                            // If it hits a non-alphanumerical character and the word buffer is not empty, then we have a word
                            else if (wordBufferCount > 0)
                                textProcessingAction(output, bufferSize, ref wordBuffer, ref wordBufferCount);
                        }
                    }
                }
            }

            return
                output;
        }
        #endregion

        #region ITokenizer Implementation
        public IEnumerable<Token> Tokenize()
        {
            // Considering tokens are always unique, a HashSet<T> will always provide 
            // a O(1) complexity if it is necessary to search for a particular token
            HashSet<Token> tokens = new HashSet<Token>();

            _ProcessText<HashSet<Token>>(tokens, _AddToken);

            return
                tokens;
        }

        public IEnumerable<KeyValuePair<string, int>> ComputeWordFrequencies()
        {
            // Considering tokens are always unique, a Dictionary<TKey, TValue> will always provide 
            // a O(1) time complexity if it is necessary to search for a particular token
            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>();

            _ProcessText<Dictionary<string, int>>(wordFrequencies, _AddWordFrequency);

            return
                wordFrequencies.OrderByDescending(pair => pair.Value);
        }
        #endregion
    }
}
