using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static IVKLogger Logger => VKSDK.Logger;

        public static async void DispatchHTTPRequest(
            string baseUri,
            Dictionary<string, string> parameters,
            Action<VKHttpResult> resultCallback)
        {

            Logger.Info(">>> VKHttpRequestHelper starting http request. baseUri = {0}; parameters = {1}", baseUri, GetAsLogString(parameters));

            try
            {

                var filter = new HttpBaseProtocolFilter {AutomaticDecompression = true};

                var httpClient = new HttpClient(filter);
                
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
                var httpClient = new HttpClient();
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
                            progressCallback(((double)progress.BytesSent * 100) / progress.TotalBytesToSend.Value);
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

        private static string GetAsLogString(Dictionary<string, string> parameters) => parameters.Aggregate("", (current, kvp) => current + (kvp.Key + " = " + kvp.Value + Environment.NewLine));
    }
}
