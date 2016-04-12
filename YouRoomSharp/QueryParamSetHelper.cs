using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace YouRoomSharp
{

    internal static class QueryParamSetHelper
    {
        internal static IDictionary<string, string> AddWhen(this IDictionary<string, string> dictionary, bool predicate, string key, string value)
        {
            if (predicate)
            {
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        internal static string ToQueryString(this IDictionary<string, string> dictionary)
        {
            return
                new FormUrlEncodedContent(dictionary.OrderBy(kvp => kvp.Key))
                    .ReadAsStringAsync()
                    .Result;
        }
    }

}
