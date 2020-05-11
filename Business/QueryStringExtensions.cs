using System;
using System.Collections.Specialized;
using System.Web;

namespace VirusTracker.Business
{
    //https://stackoverflow.com/questions/14517798/append-values-to-query-string


    public static class QueryStringExtensions
    {
        public static string AddToQueryString(this string url, params object[] keysAndValues)
        {
            return UpdateQueryString(url, q =>
            {
                for (var i = 0; i < keysAndValues.Length; i += 2)
                {
                    q.Set(keysAndValues[i].ToString(), keysAndValues[i + 1].ToString());
                }
            });
        }

        public static string RemoveFromQueryString(this string url, params string[] keys)
        {
            return UpdateQueryString(url, q =>
            {
                foreach (var key in keys)
                {
                    q.Remove(key);
                }
            });
        }


        // QueryHelpers.AddQueryString(longurl, "action", "login1")
        //QueryHelpers.AddQueryString(longurl, new Dictionary<string, string> { { "action", "login1" }, { "attempts", "11" } });


        public static string UpdateQueryString(string url, Action<NameValueCollection> func)
        {
            var urlWithoutQueryString = url.Contains('?') ? url.Substring(0, url.IndexOf('?')) : url;
            var queryString = url.Contains('?') ? url.Substring(url.IndexOf('?')) : null;
            var query = HttpUtility.ParseQueryString(queryString ?? string.Empty);

            func(query);

            return urlWithoutQueryString + (query.Count > 0 ? "?" : string.Empty) + query;
        }



    }
}
