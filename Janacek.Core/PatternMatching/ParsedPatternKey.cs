using System.Collections.Generic;

namespace Janacek
{
    public class ParsedPatternKey
    {
        private readonly List<string> _elements;

        public ParsedPatternKey(ParsedPattern parsedPattern)
        {
            _elements = new List<string>(parsedPattern.Elements.Count);
            foreach ((string key, string _) in parsedPattern.Elements)
            {
                _elements.Add(key);
            }
        }

        public IReadOnlyList<string> Elements
        {
            get
            {
                return _elements;
            }
        }
    }
}
