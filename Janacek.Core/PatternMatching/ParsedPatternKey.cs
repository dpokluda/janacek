//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Collections.Generic;

namespace Janacek
{
    /// <summary>
    /// Pattern elements keys used for proper ordering of patterns for pattern matching.
    /// </summary>
    public class ParsedPatternKey
    {
        /// <summary>
        /// Collection of pattern elements keys.
        /// </summary>
        private readonly List<string> _elements;

        /// <summary>
        /// Initializes a new instance of the Janacek.ParsedPatternKey class.
        /// </summary>
        /// <param name="parsedPattern">Parsed pattern.</param>
        public ParsedPatternKey(ParsedPattern parsedPattern)
        {
            if (parsedPattern == null)
            {
                parsedPattern = ParsedPattern.Empty;
            }
            _elements = new List<string>(parsedPattern.Elements.Count);
            foreach ((string key, string _) in parsedPattern.Elements)
            {
                _elements.Add(key);
            }
        }

        /// <summary>
        /// Gets the parsed elements keys.
        /// </summary>
        public IReadOnlyList<string> Elements
        {
            get
            {
                return _elements;
            }
        }
    }
}
