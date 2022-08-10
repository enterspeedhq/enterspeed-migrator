using System.Collections.Generic;
using System.Linq;
using Umbraco.Extensions;

namespace Umbraco10.Migrator.Extensions
{
    public static class StringExtensions
    {
        public static string ToUmbracoName(this string name, List<string> additionalWords = null)
        {
            var stringToReturn = string.Concat(name.ToFirstUpper().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            if (additionalWords != null && additionalWords.Any())
            {
                foreach (var word in additionalWords)
                {
                    stringToReturn = stringToReturn + " " + word;
                }
            }

            return stringToReturn;
        }
    }
}
