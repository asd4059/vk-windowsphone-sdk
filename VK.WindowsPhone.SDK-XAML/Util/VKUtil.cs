using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using System.Threading.Tasks;
using System.Net;


namespace VK.WindowsPhone.SDK.Util
{
    public static class VKUtil
    {
        /// <summary>
        /// Breaks key=value&key=value string to map
        /// </summary>
        /// <param name="queryString">string to explode</param>
        /// <returns>Key-value map of passed string</returns>
        public static Dictionary<string, string> ExplodeQueryString(string queryString)
        {
            var keyValuePairs = queryString.Split('&');
            var parameters = new Dictionary<string, string>(keyValuePairs.Length);

            foreach (var keyValueArray in keyValuePairs.Select(keyValueString => keyValueString.Split('=')).Where(keyValueArray => keyValueArray.Length == 2))
                parameters.Add(keyValueArray[0], keyValueArray[1]);

            return parameters;
        }

        public static string GetParamsOfQueryString(string queryString)
        {
            var indOfQ = queryString.IndexOf('?');

            if (indOfQ >= 0 && indOfQ < queryString.Length - 1)
            {
                var paramsString = queryString.Substring(indOfQ + 1);
                return paramsString;
            }

            return string.Empty;
        }

        /// <summary>
        /// Join parameters to map into string, usually query string
        /// </summary>
        /// <param name="queryArgs">Map to join</param>
        /// <param name="isUri">Indicates that value parameters must be url-encoded</param>
        /// <returns>Result query string, like k=v&k1=v=1</returns>
        /// <summary>
        /// Join parameters to map into string, usually query string
        /// </summary>
        /// <param name="queryArgs">Map to join</param>
        /// <param name="isUri">Indicates that value parameters must be url-encoded</param>
        /// <returns>Result query string, like k=v&k1=v=1</returns>
        public static string JoinParams(Dictionary<string, object> queryArgs, bool isUri = false)
        {
            var args = new List<string>(queryArgs.Count);
            args.AddRange(from entry in queryArgs let value = entry.Value select string.Format("{0}={1}", entry.Key, isUri ? WebUtility.UrlEncode(value.ToString()) : value.ToString()));
            return string.Join("&", args);
        }


        public static Dictionary<string, string> DictionaryFrom(params string[] args)
        {
            if (args.Length % 2 != 0)
                throw new Exception("Args must be paired. Last one is ignored");

            var result = new Dictionary<string, string>();
            for (var i = 0; i + 1 < args.Length; i += 2)
            {
                if (!string.IsNullOrEmpty(args[i + 1]))
                    result.Add(args[i], args[i + 1]);
            }

            return result;
        }

        /// <summary>
        /// Reads content of file and returns result as string
        /// </summary>
        /// <param name="filename">File name in IsolatedStorage</param>
        /// <returns>Contents of file</returns>
        public static string FileToString(string filename)
        {
            var text = string.Empty;

            Task.Run(async () =>
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                    text = await FileIO.ReadTextAsync(file);
                })
                .Wait();

            return text;
        }

        /// <summary>
        /// Saves passed string to file in IsolatedStorage.
        /// </summary>
        /// <param name="filename">Filename in IsolatedStorage</param>
        /// <param name="stringToWrite">String to save</param>
        public static void StringToFile(string filename, string stringToWrite)
        {
            Task.Run(async () =>
                {
                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                    await FileIO.WriteTextAsync(file, stringToWrite);

                }).Wait();
        }

        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Helper method to retreive current time from Jan 1st 1970 in milliseconds.
        /// </summary>
        /// <returns>Current time from Jan 1st 1970 in milliseconds</returns>
        public static long CurrentTimeMillis() => (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds;

        public static void ClearCookies()
        {
            var myFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            var cookieManager = myFilter.CookieManager;
            var myCookieJar = cookieManager.GetCookies(new Uri("https://vk.com"));
            foreach (var cookie in myCookieJar)
                cookieManager.DeleteCookie(cookie);

            myCookieJar = cookieManager.GetCookies(new Uri("https://login.vk.com"));
            foreach (var cookie in myCookieJar)
                cookieManager.DeleteCookie(cookie);
        }
    }
}
