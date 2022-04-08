using System.Collections.Generic;
using System.Linq;

namespace Dominisoft.Nokates.Core.Infrastructure
{
    public static class Extensions
    {
        public static Dictionary<TKey, List<TValue>> UnionValues<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, params Dictionary<TKey, List<TValue>>[] dictionaries)
        {
            var result = dictionary;

            foreach (var d in dictionaries)
            {
                foreach (var key in d.Keys)
                {
                    if (result.ContainsKey(key))
                    {
                        var value = result[key];
                        var value2 = d[key].Where(i => ! value.Contains(i));
                        value.AddRange(value2);
                        result[key] = value;

                    }
                    else
                        result.Add(key, d[key]);
                }
            }
            return result;
        }
    }
}
