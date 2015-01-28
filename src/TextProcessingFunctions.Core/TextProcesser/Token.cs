using System;

namespace TextProcessingFunctions.Core.TextProcesser
{
    /// <summary>
    /// Represents a token extracted from a text input
    /// </summary>
    public class Token : IEquatable<Token>
    {
        #region Constructors
        public Token(string content)
        {
            Content = content;
        }
        #endregion

        #region Properties
        public string Content { get; private set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            Token otherToken = obj as Token;
            bool equals = false;

            if (otherToken != null)
                equals = Equals(otherToken);

            return
                equals;

        }

        public override int GetHashCode()
        {
            return
                Content.GetHashCode();
        }

        public override string ToString()
        {
            return
                Content;
        }
        #endregion

        #region IEquatable<Token> Implementation

        public bool Equals(Token other)
        {            
            return
                other != null
                &&
                Content.Equals(other.Content);
        }

        #endregion
    }
}
