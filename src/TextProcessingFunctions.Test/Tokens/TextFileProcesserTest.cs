using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextProcessingFunctions.Core.Tokens;
using FluentAssertions;
using TextProcessingFunctions.Test.Properties;
using System.IO;
using System.Collections.Generic;

namespace TextProcessingFunctions.Test.Tokens
{
    [TestClass]
    public class TextFileProcesserTest
    {
        #region Tests
        [TestMethod]
        public void TextFileProcesser_Constructor_InvalidFilePathTest()
        {
            // Arrange                 
            string invalidTextFilePath = "textFile.dat";

            // Act/Assert
            _CreateTokenizerAction(string.Empty).ShouldThrow<ArgumentException>();
            _CreateTokenizerAction(null).ShouldThrow<ArgumentException>();
            _CreateTokenizerAction(invalidTextFilePath).ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void TextFileProcesser_Tokenize_NonExistingTestFileTest()
        {
            // Arrange
            string invalidTextFile = "nonExistingFile.txt";
            ITextProcesser tokenizer = new TextFileProcesser(invalidTextFile);
            Action action = () => tokenizer.Tokenize();

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
            tokens.Should().BeEquivalentTo(expectedTokens);
        }

        [TestMethod]
        public void TextFileProcesser_ComputeWordFrequencies_NonExistingTestFileTest()
        {
            // Arrange
            string invalidTextFile = "nonExistingFile.txt";
            ITextProcesser tokenizer = new TextFileProcesser(invalidTextFile);
            Action action = () => tokenizer.ComputeWordFrequencies();

            // Act/Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void TextFileProcesser_ComputeWordFrequencies_ValidTextFileTest()
        {
            // Arrange           
            ITextProcesser tokenizer = new TextFileProcesser(_CreateTextFile());
            IEnumerable<KeyValuePair<string, int>> wordFrequencies;
            IEnumerable<KeyValuePair<string, int>> expectedWordFrequencies = _RetrieveExpectedWordFrequencies();

            // Act
            wordFrequencies = tokenizer.ComputeWordFrequencies();

            // Assert
            wordFrequencies.Should().BeEquivalentTo(expectedWordFrequencies);
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
            string line;

            using (StringReader reader = new StringReader(Resources.TestFile_ExpectedTokens))
            {
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

        private IEnumerable<KeyValuePair<string, int>> _RetrieveExpectedWordFrequencies()
        {
            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>();
            string line;

            using (StringReader reader = new StringReader(Resources.TestFile_ExpectedWordFrequencies))
            {
                do
                {
                    line = reader.ReadLine();

                    if (line != null)
                    {
                        string[] wordData = line.Split('-');

                        wordFrequencies.Add
                        (
                            wordData[0].Trim(), 
                            int.Parse(wordData[1].Trim())
                        );
                    }

                } while (line != null);                
            }

            return
                wordFrequencies;
        } 
        #endregion
    }
}
