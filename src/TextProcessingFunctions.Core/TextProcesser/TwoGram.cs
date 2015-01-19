using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.Token1 = token1;
            this.Token2 = token2;
        }
        #endregion

        #region Properties
        public Token Token1 { get; set; }
        public Token Token2 { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            TwoGram otherTwoGram = obj as TwoGram;
            bool equals = false;

            if (otherTwoGram != null)
                equals = this.Equals(otherTwoGram);

            return
                equals;

        }

        public override int GetHashCode()
        {
            return
                this.Token1.GetHashCode() 
                +
                this.Token2.GetHashCode();
        }

        public override string ToString()
        {
            return
                this.Token1.Content + " " + this.Token2.Content;
        }
        #endregion

        #region IEquatable<Token> Implementation

        public bool Equals(TwoGram other)
        {
            return
                other != null
                &&
                (
                    (this.Token1 == null && other.Token1 == null)
                    ||
                    (this.Token1 != null && this.Token1.Equals(other.Token1))
                )
                &&
                (
                    (this.Token2 == null && other.Token2 == null)
                    ||
                    (this.Token2 != null && this.Token2.Equals(other.Token2))
                );
        }

        #endregion
    }
}
