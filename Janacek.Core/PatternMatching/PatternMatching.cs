//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Janacek
{
    /// <summary>
    /// A pattern matching.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class PatternMatching<TValue>
    {
        /// <summary>
        /// The patterns.
        /// </summary>
        /// <remarks>I have considered using a thread safe collection, like ConcurrentBag, 
        /// but that would mean that every iteration would instantiate a new list and array by calling ToList().ToArray() methods.
        /// </remarks>
        private readonly SortedList<ParsedPatternKey, List<PatternValuePair<TValue>>> _patterns;

        /// <summary>
        /// The synchronise root.
        /// </summary>
        [NonSerialized]
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Initializes a new instance of the Janacek.PatternMatching&lt;TValue&gt; class.
        /// </summary>
        public PatternMatching()
        {
            _patterns = new SortedList<ParsedPatternKey, List<PatternValuePair<TValue>>>(new ParsedPatternKeyComparer());
        }

        /// <summary>
        /// Gets the patterns.
        /// </summary>
        /// <value>
        /// The patterns.
        /// </value>
        public IReadOnlyList<PatternValuePair<TValue>> Patterns
        {
            get
            {
                var result = new List<PatternValuePair<TValue>>(_patterns.Count);
                foreach (List<PatternValuePair<TValue>> patternValuePairs in _patterns.Values)
                {
                    foreach (PatternValuePair<TValue> patternValuePair in patternValuePairs)
                    {

                        result.Add(patternValuePair);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Adds pattern.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        /// <param name="pattern">Specifies the pattern.</param>
        /// <param name="value">The value.</param>
        public void Add(string pattern, TValue value)
        {
            var parsedPattern = new ParsedPattern(pattern);
            var patternKey = new ParsedPatternKey(parsedPattern);

            if (!_patterns.TryGetValue(patternKey, out List<PatternValuePair<TValue>> patterns))
            {
                lock (_syncRoot)
                {
                    if (!_patterns.TryGetValue(patternKey, out patterns))
                    {
                        patterns = new List<PatternValuePair<TValue>>
                        {
                            new PatternValuePair<TValue>(parsedPattern, value)
                        };
                        if (!_patterns.TryAdd(patternKey, patterns))
                        {
                            throw new ArgumentException("Pattern with the same key already exists.");
                        }

                        // pattern added - return
                        return;
                    }
                    // pattern key was found (added quickly after our first TryGet - fall back to simply add
                }
            }

            patterns.Add(new PatternValuePair<TValue>(parsedPattern, value));
        }

        /// <summary>
        /// Matches the given message.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <returns>
        /// A TValue.
        /// </returns>
        public TValue Match(Message msg)
        {
            foreach ((ParsedPatternKey key, List<PatternValuePair<TValue>> patternValuePairs) in _patterns)
            {
                // all pattern keys must be present in the message
                bool allKeysMatch = true;
                foreach (string keyElement in key.Elements)
                {
                    if (!msg.ContainsKey(keyElement))
                    {
                        allKeysMatch = false;
                        break;
                    }
                }

                // if all keys are matched, then iterate over all patterns to find the first match
                if (allKeysMatch)
                {
                    foreach (PatternValuePair<TValue> patternValuePair in patternValuePairs)
                    {
                        if (patternValuePair.Pattern.IsMatch(msg))
                        {
                            return patternValuePair.Value;
                        }
                    }
                }
            }

            return default(TValue);
        }

        /// <summary>
        /// Searches for the first pattern value pair matching the given pattern string.
        /// </summary>
        /// <param name="pattern">Pattern to search for.</param>
        /// <returns>
        /// Pair with the matching parsed pattern and its current value.
        /// </returns>
        public PatternValuePair<TValue> Find(string pattern)
        {
            var parsedPattern = new ParsedPattern(pattern);
            var patternKey = new ParsedPatternKey(parsedPattern);
            var comparer = new ParsedPatternKeyComparer();
            foreach ((ParsedPatternKey key, List<PatternValuePair<TValue>> patternValuePairs) in _patterns)
            {
                if (comparer.Compare(patternKey, key) == 0)
                {
                    foreach (PatternValuePair<TValue> patternValuePair in patternValuePairs)
                    {
                        if (patternValuePair.Pattern.Equals(parsedPattern))
                        {
                            return patternValuePair;
                        }
                    }
                }
            }

            return null;
        }
    }
}
