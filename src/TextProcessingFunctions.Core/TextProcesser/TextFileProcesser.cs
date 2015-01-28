using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TextProcessingFunctions.Core.Properties;
using TextProcessingFunctions.Core.Common;

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
        }
        #endregion

        #region Properties
        private readonly string _filePath;
        #endregion

        #region Methods
        private Token _AddToken(int bufferSize, ref char[] wordBuffer, ref int wordBufferCount)
        {
            // Converts the buffer into a lowercase string
            string word = new string(wordBuffer, 0, wordBufferCount).ToLower();
            Token token = new Token(word);

            // Resets word buffer
            wordBuffer = new char[bufferSize];
            wordBufferCount = 0;

            return
                token;
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

            using (StreamReader reader = File.OpenText(_filePath))
            {
                const int bufferSize = 1024;                
                char[] buffer = new char[bufferSize];

                // A word buffer is necessary instead of storing simple indexes because a word can start in one chunk and end in another
                char[] wordBuffer = new char[bufferSize];
                int wordBufferCount = 0;

                while (!reader.EndOfStream)
                {
                    int charactersReadCount = reader.Read(buffer, 0, bufferSize);

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
                                    yield return _AddToken(bufferSize, ref wordBuffer, ref wordBufferCount);
                            }

                            // If it hits a non-alphanumerical character and the word buffer is not empty, then we have a word
                            else if (wordBufferCount > 0)
                                yield return _AddToken(bufferSize, ref wordBuffer, ref wordBufferCount);
                        }
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<Token, int>> ComputeWordFrequencies()
        {
            // Considering tokens are always unique, a Dictionary<TKey, TValue> will always provide 
            // a O(1) time complexity if it is necessary to search for a particular token
            Dictionary<Token, int> wordFrequencies = new Dictionary<Token, int>();

            foreach (var token in Tokenize())
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

            foreach (var pair in Tokenize().IterateWithNextItem())
            {
                if (pair.Item1 != null && pair.Item2 != null)
                {
                    TwoGram twoGram = new TwoGram(pair.Item1, pair.Item2);

                    if (twoGramFrequencies.ContainsKey(twoGram))
                        twoGramFrequencies[twoGram]++;
                    else
                        twoGramFrequencies.Add(twoGram, 1);
                }
            }

            return
                twoGramFrequencies.OrderByDescending(twoGramFrequency => twoGramFrequency.Value);
        }

        public IEnumerable<KeyValuePair<string, int>> ComputePalindromeFrequencies()
        {
            Dictionary<string, int> palindromeFrequencies = new Dictionary<string, int>();
            string palindromeCandidate = string.Empty;
            List<Token> tokenList = Tokenize().ToList();

            // 1st algorithm version: brute force
            // O(N^2) time complexity and O(N) spatial complexity
            // TODO: implement 2nd version with Manacher's algorithm
            for (int i = 0; i < tokenList.Count; i++)
            {
                for (int j = i; j < tokenList.Count; j++)
                {
                    palindromeCandidate += tokenList[j].Content;

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
