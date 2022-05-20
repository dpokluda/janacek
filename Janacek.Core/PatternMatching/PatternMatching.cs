using System.Collections.Generic;

namespace Janacek
{
    public class PatternMatching<TValue>
    {
        protected SortedList<ParsedPatternKey, List<PatternValuePair<TValue>>> _patterns;

        public PatternMatching()
        {
            _patterns = new SortedList<ParsedPatternKey, List<PatternValuePair<TValue>>>(new ParsedPatternKeyComparer());
        }

        public IReadOnlyList<PatternValuePair<TValue>> Patterns
        {
            get
            {
                var result = new List<PatternValuePair<TValue>>(_patterns.Count);
                foreach ((ParsedPatternKey _, List<PatternValuePair<TValue>> patternValuePairs) in _patterns)
                {
                    foreach (PatternValuePair<TValue> patternValuePair in patternValuePairs)
                    {

                        result.Add(patternValuePair);
                    }
                }

                return result;
            }
        }

        public void Add(string pattern, TValue value)
        {
            var parsedPattern = new ParsedPattern(pattern);
            var patternKey = new ParsedPatternKey(parsedPattern);

            if (!_patterns.ContainsKey(patternKey))
            {
                _patterns.Add(patternKey, new List<PatternValuePair<TValue>>());
            }
            _patterns[patternKey].Add(new PatternValuePair<TValue>(parsedPattern, value));
        }

        public TValue Match(Message msg)
        {
            foreach ((ParsedPatternKey key, List<PatternValuePair<TValue>> patternValuePairs) in _patterns)
            {
                // all pattern keys must be present in the message
                bool allKeysMatch = true;
                foreach (string keyElement in key.Elements)
                {
                    if (!msg.ContainsKey(keyElement) && keyElement != "*")
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
    }
}
