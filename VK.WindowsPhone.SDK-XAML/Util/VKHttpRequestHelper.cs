using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using VK.WindowsPhone.SDK.API;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace VK.WindowsPhone.SDK.Util
{
    public class VKHttpResult
    {
        public bool IsSucceeded { get; set; }

        public string Data { get; set; }
    }

    public static class VKHttpRequestHelper
    {
        private const int BUFFER_SIZE = 5000;

        private class RequestState
        {
            // This class stores the State of the request.           
            public StringBuilder requestData;
            public readonly List<byte> readBytes;
            public byte[] BufferRead;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public Stream streamResponse;
            public Action<VKHttpResult> resultCallback;
            public readonly DateTime startTime;
            public bool httpError;

            public RequestState()
            {
                BufferRead = new byte[BUFFER_SIZE];
                requestData = new StringBuilder("");
                readBytes = new List<byte>(1024);
                request = null;
                streamResponse = null;
                startTime = DateTime.Now;
            }
        }

        private static IVKLogger Logger => VKSDK.Logger;

        public static async void DispatchHTTPRequest(
            string baseUri,
            Dictionary<string, string> parameters,
            Action<VKHttpResult> resultCallback)
        {

            Logger.Info(">>> VKHttpRequestHelper starting http request. baseUri = {0}; parameters = {1}", baseUri, GetAsLogString(parameters));

            var queryString = ConvertDictionaryToQueryString(parameters, true);

            var requestState = new RequestState();

            try
            {
                
                var filter = new HttpBaseProtocolFilter();
                
                filter.AutomaticDecompression = true;

                var httpClient = new Windows.Web.Http.HttpClient(filter);
                
                var content = new HttpFormUrlEncodedContent(parameters);
                
                var result = await httpClient.PostAsync(new Uri(baseUri, UriKind.Absolute),
                    content);

                var resultStr = await result.Content.ReadAsStringAsync();

                SafeInvokeCallback(resultCallback, result.IsSuccessStatusCode, resultStr);
            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.DispatchHTTPRequest failed.", exc);
                SafeInvokeCallback(resultCallback, false, null);
            }
        }

        public static async void Upload(string uri, Stream data, string paramName, string uploadContentType,Action<VKHttpResult> resultCallback, Action<double> progressCallback = null, string fileName = null)
        {
            try
            {
                var httpClient = new Windows.Web.Http.HttpClient();
                var content = new HttpMultipartFormDataContent
                {
                    {new HttpStreamContent(data.AsInputStream()), paramName, fileName ?? "myDataFile"}
                };
                content.Headers.Add("Content-Type", uploadContentType);
                var postAsyncOp =  httpClient.PostAsync(new Uri(uri, UriKind.Absolute),
                    content);

                postAsyncOp.Progress = (r, progress) =>
                    {
                        if (progressCallback != null && progress.TotalBytesToSend.HasValue && progress.TotalBytesToSend > 0)
                        {
                            progressCallback(((double)progress.BytesSent * 100) / progress.TotalBytesToSend.Value);
                        }
                    };


                var result = await postAsyncOp;

                var resultStr = await result.Content.ReadAsStringAsync();

                SafeInvokeCallback(resultCallback, result.IsSuccessStatusCode, resultStr);
            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.Upload failed.", exc);
                SafeInvokeCallback(resultCallback, false, null);
            }
        }

        private static void SafeInvokeCallback(Action<VKHttpResult> action, bool p, string stringContent)
        {          
            try
            {
                action(new VKHttpResult { IsSucceeded =  p, Data =  stringContent});
            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.SafeInvokeCallback failed.", exc);
            }
        }

        private static string ConvertDictionaryToQueryString(Dictionary<string, string> parameters, bool escapeString)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var kvp in parameters.Where(kvp => kvp.Key != null && kvp.Value != null))
            {
                if (sb.Length > 0)
                    sb = sb.Append("&");

                var valueStr = escapeString ? Uri.EscapeDataString(kvp.Value) : kvp.Value;

                sb = sb.AppendFormat(
                    "{0}={1}",
                    kvp.Key,
                    valueStr);
            }

            return sb.ToString();
        }

        private static string GetAsLogString(Dictionary<string, string> parameters) => parameters.Aggregate("", (current, kvp) => current + (kvp.Key + " = " + kvp.Value + Environment.NewLine));
    }
}
