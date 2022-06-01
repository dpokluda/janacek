//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

namespace Janacek
{
    /// <summary>
    /// A pattern with a value pair.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class PatternValuePair<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the Janacek.PatternValuePair&lt;TValue&gt; class.
        /// </summary>
        /// <param name="pattern">Specifies the pattern.</param>
        /// <param name="value">The value associated with the pattern.</param>
        public PatternValuePair(ParsedPattern pattern, TValue value)
        {
            Pattern = pattern;
            Value = value;
        }

        /// <summary>
        /// The pattern.
        /// </summary>
        public ParsedPattern Pattern;

        /// <summary>
        /// The value associated with the pattern
        /// </summary>
        public TValue Value;
    }
}
