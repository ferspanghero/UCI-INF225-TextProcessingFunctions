using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextProcessingFunctions.Core.Common;
using FluentAssertions;

namespace TextProcessingFunctions.Test.Common
{
    [TestClass]
    public class ExtensionsTest
    {
        #region Tests
        [TestMethod]
        public void Extensions_IEnumerable_IterateWithNextItemTest()
        {
            // Arrange                 
            List<char> list = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f' };            
            List<Tuple<char, char>> expectedResult =
                new List<Tuple<char, char>>()
                {
                    new Tuple<char, char>('a', 'b'),
                    new Tuple<char, char>('b', 'c'),
                    new Tuple<char, char>('c', 'd'),
                    new Tuple<char, char>('d', 'e'),
                    new Tuple<char, char>('e', 'f'),
                    new Tuple<char, char>('f', default(char))
                };

            // Act
            var result = list.IterateWithNextItem();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
        #endregion
    }
}
