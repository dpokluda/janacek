namespace Janacek
{
    public class PatternValuePair<TValue>
    {
        public PatternValuePair(ParsedPattern pattern, TValue value)
        {
            Pattern = pattern;
            Value = value;
        }

        public ParsedPattern Pattern;
        public TValue Value;
    }
}
