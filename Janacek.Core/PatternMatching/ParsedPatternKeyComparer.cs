using System.Collections.Generic;

namespace Janacek
{
    public class ParsedPatternKeyComparer : IComparer<ParsedPatternKey>
    {
        public int Compare(ParsedPatternKey x, ParsedPatternKey y)
        {
            var result = -(x.Elements.Count.CompareTo(y.Elements.Count));
            if (result == 0)
            {
                for (int index = 0; index < x.Elements.Count; index++)
                {
                    result = x.Elements[index].CompareTo(y.Elements[index]);
                    if (result != 0)
                    {
                        if (x.Elements[index] == "*")
                        {
                            return 1;
                        }
                        else if (y.Elements[index] == "*")
                        {
                            return -1;
                        }
                        break;
                    }
                }
            }

            return result;
        }
    }
}
