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
    public class TextFileTokenizerTest
    {
        #region Tests
        [TestMethod]
        public void TextFileTokenizer_Constructor_InvalidFilePathTest()
        {
            // Arrange                 
            string invalidTextFilePath = "textFile.dat";

            // Act/Assert
            _CreateTokenizerAction(string.Empty).ShouldThrow<ArgumentException>();
            _CreateTokenizerAction(null).ShouldThrow<ArgumentException>();
            _CreateTokenizerAction(invalidTextFilePath).ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void TextFileTokenizer_Tokenize_NonExistingTestFileTest()
        {
            // Arrange
            string invalidTextFile = "nonExistingFile.txt";
            ITokenizer tokenizer = new TextFileTokenizer(invalidTextFile);
            Action action = () => tokenizer.Tokenize();

            // Act/Assert
            action.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void TextFileTokenizer_Tokenize_ValidTextFileTest()
        {
            // Arrange           
            ITokenizer tokenizer = new TextFileTokenizer(_CreateTextFile());
            IEnumerable<Token> tokens;

            // Act
            tokens = tokenizer.Tokenize();

            // Assert
            tokens.Should().NotBeNullOrEmpty();
        }        
        #endregion

        #region Auxiliar
        private Action _CreateTokenizerAction(string textFilePath)
        {
            return
                () => new TextFileTokenizer(textFilePath);
        }

        private string _CreateTextFile()
        {
            string path = Path.ChangeExtension(Path.GetTempFileName(), "txt");

            using (StreamWriter writer = File.CreateText(path))
                writer.Write(Resources.TestTextFile);
        
            return
                path;
        }
        #endregion
    }
}
