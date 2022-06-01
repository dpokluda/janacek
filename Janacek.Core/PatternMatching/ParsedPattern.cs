//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Collections.Generic;

namespace Janacek
{
    /// <summary>
    /// Parsed pattern used for pattern matching.
    /// </summary>
    public class ParsedPattern
    {
        /// <summary>
        /// Any value pattern.
        /// </summary>
        public const string AnyValue = "*";

        /// <summary>
        /// Predefined/shared empty parsed pattern.
        /// </summary>
        public static readonly ParsedPattern Empty = new ParsedPattern(null);

        /// <summary>
        /// Predefined/shared empty parsed pattern.
        /// </summary>
        private static readonly List<KeyValuePair<string, string>> EmptyPattern = new List<KeyValuePair<string, string>>(0);

        /// <summary>
        /// Pattern elements separator.
        /// </summary>
        private static readonly char[] ElementsSeparator = { ',' };

        /// <summary>
        /// Pattern element parts separator.
        /// </summary>
        private static readonly char[] PartsSeparator = { ':' };

        /// <summary>
        /// Parsed pattern elements (key=value).
        /// </summary>
        private readonly List<KeyValuePair<string, string>> _elements;

        /// <summary>
        /// Input pattern.
        /// </summary>
        private readonly string _pattern;

        /// <summary>
        /// Initializes a new instance of the Janacek.ParsedPattern class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public ParsedPattern(string pattern)
        {
            _elements = Parse(pattern);
            _pattern = pattern;
        }

        /// <summary>
        /// Gets the parsed pattern elements.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, string>> Elements
        {
            get
            {
                return _elements;
            }
        }

        /// <summary>
        /// Determines whether the specified message matches the parsed pattern.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <c>True</c> if the message matches the pattern; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(Message message)
        {
            foreach ((string key, string value) in _elements)
            {
                if (!message.TryGetValue(key, out object elementValue))
                {
                    return false;
                }

                if (value != AnyValue && elementValue.ToString() != value)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse the specified pattern (split into elements and element parts).
        /// </summary>
        /// <exception cref="PatternParsingException">Thrown when a pattern parsing error condition occurs.</exception>
        /// <param name="pattern">Specifies the pattern.</param>
        /// <returns>
        /// Collection of pattern elements.
        /// </returns>
        private List<KeyValuePair<string, string>> Parse(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return EmptyPattern;
            }

            var elements = new List<KeyValuePair<string, string>>();
            foreach (string element in pattern.Split(ElementsSeparator))
            {
                if (string.IsNullOrWhiteSpace(element))
                {
                    continue;
                }

                var parts = element.Split(PartsSeparator);
                if (parts.Length != 2)
                {
                    throw new PatternParsingException();
                }

                string key = parts[0].Trim().ToLowerInvariant();
                string value = parts[1].Trim().ToLowerInvariant();
                elements.Add(new KeyValuePair<string, string>(key, value));
            }

            return elements;
        }

        /// <summary>
        /// Returns a string that represents the current pattern.
        /// </summary>
        /// <returns>
        /// A string that represents the current pattern.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            return _pattern;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current parsed pattern.
        /// </summary>
        /// <param name="obj">The object to compare with the current parsed pattern.</param>
        /// <returns>
        /// <c>True</c> if the specified parsed pattern  is equal to the current parsed pattern; otherwise, false.
        /// </returns>
        /// <seealso cref="M:System.Object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            var other = obj as ParsedPattern;

            // if other is null then they are different
            if (other == null)
            {
                return false;
            }
            // if pointers are the same then they are same
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            // if number of pattern elements is different then they are different
            if (this.Elements.Count != other.Elements.Count)
            {
                return false;
            }
            // compare each element
            for (var index = 0; index < this.Elements.Count; index++)
            {
                // if something is different then they are different
                if (this.Elements[index].Key != other.Elements[index].Key ||
                    this.Elements[index].Value != other.Elements[index].Value)
                {
                    return false;
                }
            }
            // all checks have passed, they must be the same then
            return true;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <seealso cref="M:System.Object.GetHashCode()"/>
        public override int GetHashCode()
        {
            // TODO: proper implementation
            return base.GetHashCode();
        }
    }
}
