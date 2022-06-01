//-------------------------------------------------------------------------
// Copyright (c) David Pokluda. All rights reserved.
//-------------------------------------------------------------------------

using System.Collections.Generic;

namespace Janacek
{
    /// <summary>
    /// A custom comparer for proper ordering of parsed patterns.
    /// Pattern with more elements is higher than pattern with less elements specified. 
    /// When two patterns have the same number of elements, then order alphabetically by element keys.
    /// </summary>
    /// <seealso cref="T:System.Collections.Generic.IComparer{Janacek.ParsedPatternKey}"/>
    public class ParsedPatternKeyComparer : IComparer<ParsedPatternKey>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">First pattern to compare.</param>
        /// <param name="y">Second pattern to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x">x</paramref> and <paramref name="y">y</paramref>, as shown in the following table.  
        /// Value less than zero means that the first pattern is smaller than the second pattern. Value zero means 
        /// that the two patterns are same with respect to ordering. Value greater than zero means that the first
        /// pattern is greater than the second pattern.
        /// </returns>
        /// <seealso cref="M:System.Collections.Generic.IComparer{Janacek.ParsedPatternKey}.Compare(ParsedPatternKey,ParsedPatternKey)"/>
        public int Compare(ParsedPatternKey x, ParsedPatternKey y)
        {
            if (x == y)
            {
                return 0;
            }

            // null checking
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }

            // number of elements - more specific means better (smaller) pattern
            var result = -(x.Elements.Count.CompareTo(y.Elements.Count));
            if (result == 0)
            {
                // when the number of elements is the same then use alphabetic ordering
                for (int index = 0; index < x.Elements.Count; index++)
                {
                    // compare each element alphabetically
                    result = x.Elements[index].CompareTo(y.Elements[index]);
                    if (result != 0)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
