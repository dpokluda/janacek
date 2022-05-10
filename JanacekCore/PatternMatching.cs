using System.Collections.Generic;

namespace JanacekClient
{
    public class PatternMatching
    {
        private static readonly List<KeyValuePair<string, string>> EmptyPattern = new List<KeyValuePair<string, string>>(0);
        private static readonly char[] ElementsSeparator = new char[] { ',' };
        private static readonly char[] PartsSeparator = new char[] { ':' };

        protected SortedList<ParsedPatternKey, List<ParsedPatternActionPair>> _patterns;

        public PatternMatching()
        {
            _patterns = new SortedList<ParsedPatternKey, List<ParsedPatternActionPair>>(new ParsedPatternKeyComparer());
        }

        public void Add(string pattern, object action)
        {
            var parsed = ParsePattern(pattern);

            var parsedPatternKey = new ParsedPatternKey(parsed);
            var parsedPattern = new ParsedPattern(parsed);
            if (!_patterns.ContainsKey(parsedPatternKey))
            {
                _patterns.Add(parsedPatternKey, new List<ParsedPatternActionPair>());
            }
            _patterns[parsedPatternKey].Add(new ParsedPatternActionPair(parsedPattern, action));
        }

        public object Match(Message msg)
        {
            foreach ((ParsedPatternKey patternKey, List<ParsedPatternActionPair> patternList) in _patterns)
            {
                // all pattern keys must be present in the message
                bool allKeysMatch = true;
                foreach (string key in patternKey.Keys)
                {
                    if (!msg.ContainsKey(key))
                    {
                        allKeysMatch = false;
                        break;
                    }
                }

                // if all keys are matched, then iterate over all patterns to find the first match
                if (allKeysMatch)
                {
                    foreach (ParsedPatternActionPair parsedPatternActionPair in patternList)
                    {
                        if (parsedPatternActionPair.ParsedPattern.IsMatch(msg))
                        {
                            return parsedPatternActionPair.Action;
                        }
                    }
                }
            }

            return null;
        }

        private List<KeyValuePair<string, string>> ParsePattern(string pattern)
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

                var key = parts[0].Trim();
                var value = parts[1].Trim();
                elements.Add(new KeyValuePair<string, string>(key, value));
            }

            return elements;
        }
    }

    public class ParsedPatternActionPair
    {
        public ParsedPatternActionPair(ParsedPattern parsedPattern, object action)
        {
            ParsedPattern = parsedPattern;
            Action = action;
        }

        public ParsedPattern ParsedPattern;
        public object Action;
    }

    public class ParsedPattern
    {
        protected List<KeyValuePair<string, string>> _elements;

        public IReadOnlyList<KeyValuePair<string, string>> Elements
        {
            get
            {
                return _elements;
            }
        }

        public ParsedPattern(List<KeyValuePair<string, string>> elements)
        {
            _elements = elements;
        }

        public bool IsMatch(Message message)
        {
            foreach ((string key, string value) in _elements)
            {
                if (!message.ContainsKey(key))
                {
                    return false;
                }

                if ((string)message[key] != value)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class ParsedPatternKey
    {
        protected List<string> _keys;

        public IReadOnlyList<string> Keys
        {
            get
            {
                return _keys;
            }
        }

        public ParsedPatternKey(List<KeyValuePair<string, string>> elements)
        {
            _keys = new List<string>(elements.Count);
            foreach ((string key, string _) in elements)
            {
                _keys.Add(key);
            }
        }
    }

    public class ParsedPatternKeyComparer : IComparer<ParsedPatternKey>
    {
        public int Compare(ParsedPatternKey x, ParsedPatternKey y)
        {
            var result = -(x.Keys.Count.CompareTo(y.Keys.Count));
            if (result == 0)
            {
                for (int index = 0; index < x.Keys.Count; index++)
                {
                    result = x.Keys[index].CompareTo(y.Keys[index]);
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
