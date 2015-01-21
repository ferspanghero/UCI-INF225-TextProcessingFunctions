using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly string _filePath;
        private readonly List<Token> _indistinctTokenList;
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

        private bool _IsPalindrome(string text)
        {
            // Empty, null and one letter strings are not considered palindromes
            if (string.IsNullOrEmpty(text) || text.Length == 1)
                return false;

            int lengthChecked = text.Length / 2;

            // Only iterates half of the text
            for (int i = 0; i < lengthChecked; i++)
            {
                // Checks a character with its opposite counterpart from the end of the text
                if (text[i] != text[text.Length - i - 1])
                    return false;
            }

            return
                true;
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
                const int bufferSize = 1024;
                int charactersReadCount;
                char[] buffer = new char[bufferSize];

                // A word buffer is necessary instead of storing simple indexes because a word can start in one chunk and end in another
                char[] wordBuffer = new char[bufferSize];
                int wordBufferCount = 0;

                while (!reader.EndOfStream)
                {
                    charactersReadCount = reader.Read(buffer, 0, bufferSize);

                    if (charactersReadCount > 0)
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

            if (_indistinctTokenList != null)
                foreach (var token in _indistinctTokenList)
                {
                    // If it tries to add a token that already exists, we should increment the existing token frequency number instead
                    if (wordFrequencies.ContainsKey(token))
                        wordFrequencies[token]++;
                    else
                        wordFrequencies.Add(token, 1);
                }

            return
                wordFrequencies.OrderByDescending(wordFrequency => wordFrequency.Value);
        }

        public IEnumerable<KeyValuePair<TwoGram, int>> ComputeTwoGramFrequencies()
        {
            Dictionary<TwoGram, int> twoGramFrequencies = new Dictionary<TwoGram, int>();
            TwoGram twoGram;

            // If there are no processed tokens, try to tokenize the input text file
            if (_indistinctTokenList == null || _indistinctTokenList.Count == 0)
                Tokenize();

            if (_indistinctTokenList != null)
                for (int i = 0; i < _indistinctTokenList.Count - 1; i++)
                {
                    twoGram = new TwoGram(_indistinctTokenList[i], _indistinctTokenList[i + 1]);

                    if (twoGramFrequencies.ContainsKey(twoGram))
                        twoGramFrequencies[twoGram]++;
                    else
                        twoGramFrequencies.Add(twoGram, 1);
                }

            return
                twoGramFrequencies.OrderByDescending(twoGramFrequency => twoGramFrequency.Value);
        }

        public IEnumerable<KeyValuePair<string, int>> ComputePalindromeFrequencies()
        {
            Dictionary<string, int> palindromeFrequencies = new Dictionary<string, int>();
            string palindromeCandidate = string.Empty;

            // If there are no processed tokens, try to tokenize the input text file
            if (_indistinctTokenList == null || _indistinctTokenList.Count == 0)
                Tokenize();

            if (_indistinctTokenList != null)
                // This algorithm runs in O(N^2) time complexity on any case
                for (int i = 0; i < _indistinctTokenList.Count; i++)
                {
                    for (int j = i; j < _indistinctTokenList.Count; j++)
                    {
                        palindromeCandidate += _indistinctTokenList[j].Content;

                        if (_IsPalindrome(palindromeCandidate))
                        {
                            if (palindromeFrequencies.ContainsKey(palindromeCandidate))
                                palindromeFrequencies[palindromeCandidate]++;
                            else
                                palindromeFrequencies.Add(palindromeCandidate, 1);
                        }
                    }

                    palindromeCandidate = string.Empty;
                }

            return
                palindromeFrequencies.OrderByDescending(palindromeFrequency => palindromeFrequency.Value);
        }
        #endregion
    }
}
