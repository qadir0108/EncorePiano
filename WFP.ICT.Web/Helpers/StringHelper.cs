using System.Linq;

namespace WFP.ICT.Web.Helpers
{
    public class StringHelper
    {
        public static bool Contains(string Source, string ToCheck)
        {
            if (string.IsNullOrEmpty(Source)) return false;
            return Source.ToLowerInvariant().Contains(ToCheck.ToLowerInvariant());
        }

        public static string URLSubstring(string Source)
        {
            var parts = Source.Split("//".ToCharArray());
            return string.Join("/", parts.ToList().Skip(parts.Length - 3));
        }
    }
}