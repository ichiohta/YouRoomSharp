using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace YouRoomSharp
{

    internal static class QueryParamSetHelper
    {
        internal static IDictionary<string, string> AddOrUpdateWhen(this IDictionary<string, string> dictionary, bool predicate, string key, string value)
        {
            if (predicate)
            {
                dictionary.AddOrUpdate(key, value);
            }

            return dictionary;
        }

        internal static IDictionary<string, string> AddOrUpdate(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);

            return dictionary;
        }

        internal static string ToQueryString(this IDictionary<string, string> dictionary)
        {
            return
                dictionary.ToFormUrlEncodedContent()
                    .ReadAsStringAsync()
                    .Result;
        }

        internal static FormUrlEncodedContent ToFormUrlEncodedContent(this IDictionary<string, string> dictionary)
        {
            return new FormUrlEncodedContent(dictionary.OrderBy(kvp => kvp.Key));
        }
    }

}
