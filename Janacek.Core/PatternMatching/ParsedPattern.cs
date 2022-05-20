using System.Collections.Generic;

namespace Janacek
{
    public class ParsedPattern
    {
        private static readonly List<KeyValuePair<string, string>> EmptyPattern = new List<KeyValuePair<string, string>>(0);
        private static readonly char[] ElementsSeparator = { ',' };
        private static readonly char[] PartsSeparator = { ':' };
        private static string AllValues = "*";

        protected List<KeyValuePair<string, string>> _elements;
        protected string _pattern;

        public ParsedPattern(string pattern)
        {
            _elements = Parse(pattern);
            _pattern = pattern;
        }

        public IReadOnlyList<KeyValuePair<string, string>> Elements
        {
            get
            {
                return _elements;
            }
        }

        public bool IsMatch(Message msg)
        {
            foreach ((string key, string value) in _elements)
            {
                if (!msg.ContainsKey(key) && key != AllValues)
                {
                    return false;
                }

                if (value != "*" && msg[key].ToString() != value)
                {
                    return false;
                }
            }

            return true;
        }

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
                    if (parts.Length == 1 && element.Trim() == AllValues)
                    {
                        elements.Add(new KeyValuePair<string, string>(AllValues, AllValues));
                        continue;
                    }
                    else
                    {
                        throw new PatternParsingException();
                    }
                }

                var key = parts[0].Trim().ToLowerInvariant();
                var value = parts[1].Trim().ToLowerInvariant();
                elements.Add(new KeyValuePair<string, string>(key, value));
            }

            return elements;
        }

        public override string ToString()
        {
            return _pattern;
        }
    }
}
