using System.Collections.Generic;
using System.Linq;

namespace Pertuk.Business.Extensions.StringExt
{
    public static class StringExtensions
    {
        public static string ReplaceRange(this string textData, Dictionary<string, string> values)
        {
            return values.Aggregate(textData, (current, replacement) => current.Replace(replacement.Key, replacement.Value));
        }
    }
}