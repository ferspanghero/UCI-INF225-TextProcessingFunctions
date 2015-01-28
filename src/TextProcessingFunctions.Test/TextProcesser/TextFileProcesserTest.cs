using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextProcessingFunctions.Core.TextProcesser;
using TextProcessingFunctions.Test.Properties;

namespace TextProcessingFunctions.Test.TextProcesser
{
    [TestClass]
    public class TextFileProcesserTest
    {
        #region Tests
        [TestMethod]
        public void TextFileProcesser_Constructor_InvalidFilePathTest()
        {
            // Arrange                 
            const string invalidTextFilePath = "textFile.dat";

            // Act/Assert
            _CreateTokenizerAction(string.Empty).ShouldThrow<ArgumentException>();
            _CreateTokenizerAction(null).ShouldThrow<ArgumentException>();
            _CreateTokenizerAction(invalidTextFilePath).ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void TextFileProcesser_Tokenize_NonExistingTestFileTest()
        {
            // Arrange
            const string invalidTextFile = "nonExistingFile.txt";
            ITextProcesser tokenizer = new TextFileProcesser(invalidTextFile);

            // The ToList is necessary due to IEnumerable lazy loading
            Action action = () => tokenizer.Tokenize().ToList();

            // Act/Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void TextFileProcesser_Tokenize_ValidTextFileTest()
        {
            // Arrange           
            ITextProcesser tokenizer = new TextFileProcesser(_CreateTextFile());
            IEnumerable<Token> tokens;
            IEnumerable<Token> expectedTokens = _RetrieveExpectedTokens();

            // Act
            tokens = tokenizer.Tokenize();

            // Assert            
            tokens.Distinct().Should().BeEquivalentTo(expectedTokens);
        }        

        [TestMethod]
        public void TextFileProcesser_ComputeWordFrequencies_ValidTextFileTest()
        {
            // Arrange           
            ITextProcesser tokenizer = new TextFileProcesser(_CreateTextFile());
            IEnumerable<KeyValuePair<Token, int>> wordFrequencies;
            IEnumerable<KeyValuePair<Token, int>> expectedWordFrequencies = _RetrieveExpectedWordFrequencies();

            // Act
            wordFrequencies = tokenizer.ComputeWordFrequencies();

            // Assert
            wordFrequencies.Should().BeInDescendingOrder(wordFrequency => wordFrequency.Value);
            expectedWordFrequencies.Should().BeInDescendingOrder(wordFrequency => wordFrequency.Value);
            wordFrequencies.Should().BeEquivalentTo(expectedWordFrequencies);
        }        

        [TestMethod]
        public void TextFileProcesser_ComputeTwoGramFrequencies_ValidTextFileTest()
        {
            // Arrange           
            ITextProcesser tokenizer = new TextFileProcesser(_CreateTextFile());
            IEnumerable<KeyValuePair<TwoGram, int>> twoGramsFrequencies;
            IEnumerable<KeyValuePair<TwoGram, int>> expectedTwoGramsFrequencies = _RetrieveExpectedTwoGramFrequencies();

            // Act
            twoGramsFrequencies = tokenizer.ComputeTwoGramFrequencies();

            // Assert
            twoGramsFrequencies.Should().BeInDescendingOrder(twoGramFrequency => twoGramFrequency.Value);
            expectedTwoGramsFrequencies.Should().BeInDescendingOrder(twoGramFrequency => twoGramFrequency.Value);
            twoGramsFrequencies.Should().BeEquivalentTo(expectedTwoGramsFrequencies);
        }

        [TestMethod]
        public void TextFileProcesser_ComputePalindromeFrequencies_ValidTextFileTest()
        {
            // Arrange           
            ITextProcesser tokenizer = new TextFileProcesser(_CreateTextFile());
            IEnumerable<KeyValuePair<string, int>> palindromeFrequencies;
            IEnumerable<KeyValuePair<string, int>> expectedpalindromeFrequencies = _RetrieveExpectedPalindromeFrequencies();

            // Act
            palindromeFrequencies = tokenizer.ComputePalindromeFrequencies();

            // Assert
            palindromeFrequencies.Should().BeInDescendingOrder(palindromeFrequency => palindromeFrequency.Value);
            expectedpalindromeFrequencies.Should().BeInDescendingOrder(palindromeFrequency => palindromeFrequency.Value);
            palindromeFrequencies.Should().BeEquivalentTo(expectedpalindromeFrequencies);
        }                              
        #endregion

        #region Auxiliar
        private Action _CreateTokenizerAction(string textFilePath)
        {
            return
                () => new TextFileProcesser(textFilePath);
        }

        private string _CreateTextFile()
        {
            string path = Path.ChangeExtension(Path.GetTempFileName(), "txt");

            using (StreamWriter writer = File.CreateText(path))
                writer.Write(Resources.TestTextFile);
        
            return
                path;
        }

        private IEnumerable<Token> _RetrieveExpectedTokens()
        {
            HashSet<Token> tokens = new HashSet<Token>();

            using (StringReader reader = new StringReader(Resources.TestFile_ExpectedTokens))
            {
                string line;

                do
                {
                    line = reader.ReadLine();

                    if (line != null)
                        tokens.Add(new Token(line));

                } while (line != null);
            }

            return
                tokens;
        }  

        private IEnumerable<KeyValuePair<Token, int>> _RetrieveExpectedWordFrequencies()
        {
            Dictionary<Token, int> wordFrequencies = new Dictionary<Token, int>();

            using (StringReader reader = new StringReader(Resources.TestFile_ExpectedWordFrequencies))
            {
                string line;

                do
                {
                    line = reader.ReadLine();

                    if (line != null)
                    {
                        // This method was conceived in the simplest and quickest way possible and expects a
                        // hard-coded text file with the calculated word frequencies for the input text file
                        string[] wordData = line.Split('-');

                        wordFrequencies.Add
                        (
                            new Token(wordData[0].Trim()), 
                            int.Parse(wordData[1].Trim())
                        );
                    }

                } while (line != null);
            }

            return
                wordFrequencies;
        }

        private IEnumerable<KeyValuePair<TwoGram, int>> _RetrieveExpectedTwoGramFrequencies()
        {
            Dictionary<TwoGram, int> twoGramsFrequencies = new Dictionary<TwoGram, int>();

            using (StringReader reader = new StringReader(Resources.TestFile_ExpectedTwoGramFrequencies))
            {
                string line;

                do
                {
                    line = reader.ReadLine();

                    if (line != null)
                    {
                        // This method was conceived in the simplest and quickest way possible and expects
                        // a hard-coded text file with the calculated 2-grams for the input text file
                        string[] twoGramsData = line.Split('-');
                        string[] tokens = twoGramsData[0].Split(' ');

                        twoGramsFrequencies.Add
                        (
                            new TwoGram
                            (
                                new Token(tokens[0].Trim()), 
                                new Token(tokens[1].Trim())
                            ),
                            int.Parse(twoGramsData[1].Trim())
                        );
                    }

                } while (line != null);
            }

            return
                twoGramsFrequencies;
        }

        private IEnumerable<KeyValuePair<string, int>> _RetrieveExpectedPalindromeFrequencies()
        {
            Dictionary<string, int> palindromeFrequencies = new Dictionary<string, int>();

            using (StringReader reader = new StringReader(Resources.TestFile_ExpectedPalindromeFrequencies))
            {
                string line;

                do
                {
                    line = reader.ReadLine();

                    if (line != null)
                    {
                        // This method was conceived in the simplest and quickest way possible and expects
                        // a hard-coded text file with the calculated 2-grams for the input text file
                        string[] palindromeData = line.Split('-');

                        palindromeFrequencies.Add
                        (
                            palindromeData[0].Trim(),
                            int.Parse(palindromeData[1].Trim())
                        );
                    }

                } while (line != null);
            }

            return
                palindromeFrequencies;
        }
        #endregion
    }
}
