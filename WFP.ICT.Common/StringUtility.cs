using System;

namespace WFP.ICT.Common
{
    public static class StringUtility
    {
        public static bool Contains(string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}
