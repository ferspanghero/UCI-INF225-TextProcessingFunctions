using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextProcessingFunctions.Core.Tokens
{
    /// <summary>
    /// Represents a token extracted from a text input
    /// </summary>
    public class Token : IEquatable<Token>
    {
        #region Constructors
        public Token(string content)
        {
            this.Content = content;
        }
        #endregion

        #region Properties
        public string Content { get; set; }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            Token otherToken = obj as Token;
            bool equals = false;

            if (otherToken != null)
                equals = this.Equals(otherToken);

            return
                equals;

        }

        public override int GetHashCode()
        {
            return
                this.Content.GetHashCode();
        }

        public override string ToString()
        {
            return
                this.Content;
        }
        #endregion

        #region IEquatable<Token> Implementation

        public bool Equals(Token other)
        {            
            return
                other != null
                &&
                this.Content.Equals(other.Content);
        }

        #endregion
    }
}
