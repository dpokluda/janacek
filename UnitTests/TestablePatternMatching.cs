using System.Collections.Generic;
using JanacekClient;

namespace UnitTests
{
    public class TestablePatternMatching : PatternMatching
    {
        public IReadOnlyList<KeyValuePair<ParsedPattern, object>> Patterns
        {
            get
            {
                var result = new List<KeyValuePair<ParsedPattern, object>>(_patterns.Count);
                foreach ((ParsedPatternKey _, List<ParsedPatternActionPair> patternList) in _patterns)
                {
                    foreach (ParsedPatternActionPair parsedPatternActionPair in patternList)
                    {
                        result.Add(new KeyValuePair<ParsedPattern, object>(parsedPatternActionPair.ParsedPattern, parsedPatternActionPair.Action));
                    }
                }

                return result;
            }
        }

    }
}
