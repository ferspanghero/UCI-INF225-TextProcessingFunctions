using System;

namespace TextProcessingFunctions.Core.TextProcesser
{
    /// <summary>
    /// Represents a token extracted from a text input
    /// </summary>
    public class TwoGram : IEquatable<TwoGram>
    {
        #region Constructors
        public TwoGram(Token token1, Token token2)
        {
            if (token1 == null)
                throw new ArgumentNullException("token1");

            if (token2 == null)
                throw new ArgumentNullException("token2");

            Token1 = token1;
            Token2 = token2;
        }
        #endregion

        #region Properties
        public Token Token1 { get; private set; }
        public Token Token2 { get; private set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            TwoGram otherTwoGram = obj as TwoGram;
            bool equals = false;

            if (otherTwoGram != null)
                equals = Equals(otherTwoGram);

            return
                equals;

        }

        public override int GetHashCode()
        {
            return
                Token1.GetHashCode() 
                +
                Token2.GetHashCode();
        }

        public override string ToString()
        {
            return
                Token1.Content + " " + Token2.Content;
        }
        #endregion

        #region IEquatable<Token> Implementation

        public bool Equals(TwoGram other)
        {
            return
                other != null
                &&
                (
                    (Token1 == null && other.Token1 == null)
                    ||
                    (Token1 != null && Token1.Equals(other.Token1))
                )
                &&
                (
                    (Token2 == null && other.Token2 == null)
                    ||
                    (Token2 != null && Token2.Equals(other.Token2))
                );
        }

        #endregion
    }
}
