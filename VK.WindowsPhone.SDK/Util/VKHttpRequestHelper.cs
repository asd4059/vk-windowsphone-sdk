﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using VK.WindowsPhone.SDK.API;

#if SILVERLIGHT
using VK.WindowsPhone.SDK.API.Networking;

#else


using Windows.Web.Http;
using Windows.Web.Http.Filters;

#endif

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
#if SILVERLIGHT

                requestState.resultCallback = resultCallback;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUri);

                requestState.request = httpWebRequest;

                httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                httpWebRequest.Method = "POST";

                httpWebRequest.BeginGetRequestStream(ar =>
                {
                    try
                    {
                        var requestStream =
                            httpWebRequest.EndGetRequestStream(ar);

                        using (var sw = new StreamWriter(requestStream))
                        {
                            sw.Write(queryString);
                        }

                        httpWebRequest.BeginGetCompressedResponse(
                            new AsyncCallback(ResponseCallback), requestState);
                    }
                    catch (Exception exc)
                    {
                        Logger.Error("VKHttpRequestHelper.DispatchHTTPRequest failed.", exc);
                        SafeClose(requestState);
                        SafeInvokeCallback(requestState.resultCallback, false, null);
                    }

                },
                null);

#else
                
                var filter = new HttpBaseProtocolFilter();
                
                filter.AutomaticDecompression = true;

                var httpClient = new Windows.Web.Http.HttpClient(filter);
                
                HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(parameters);
                
                var result = await httpClient.PostAsync(new Uri(baseUri, UriKind.Absolute),
                    content);

                var resultStr = await result.Content.ReadAsStringAsync();

                SafeInvokeCallback(resultCallback, result.IsSuccessStatusCode, resultStr);

#endif

            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.DispatchHTTPRequest failed.", exc);
#if SILVERLIGHT
                SafeClose(requestState);
                SafeInvokeCallback(requestState.resultCallback, false, null);
#else
                SafeInvokeCallback(resultCallback, false, null);
#endif

            }
        }

        public static async void Upload(string uri, Stream data, string paramName, string uploadContentType,Action<VKHttpResult> resultCallback, Action<double> progressCallback = null, string fileName = null)
        {
#if SILVERLIGHT
            var rState = new RequestState();
            rState.resultCallback = resultCallback;
#endif
            try
            {
#if SILVERLIGHT
                var request = (HttpWebRequest)WebRequest.Create(uri);
            
                request.AllowWriteStreamBuffering = false;
                rState.request = request;
                request.Method = "POST";
                var formDataBoundary = $"----------{Guid.NewGuid():N}";
                var contentType = "multipart/form-data; boundary=" + formDataBoundary;
                request.ContentType = contentType;
                request.CookieContainer = new CookieContainer();

                var header = $"--{formDataBoundary}\r\nContent-Disposition: form-data; name=\"{paramName}\"; filename=\"{fileName ?? "myDataFile"}\";\r\nContent-Type: {uploadContentType}\r\n\r\n";

                var footer = "\r\n--" + formDataBoundary + "--\r\n";

                request.ContentLength = Encoding.UTF8.GetByteCount(header) + data.Length + Encoding.UTF8.GetByteCount(footer);

                request.BeginGetRequestStream(ar =>
                {
                    try
                    {
                        var requestStream = request.EndGetRequestStream(ar);

                        requestStream.Write(Encoding.UTF8.GetBytes(header), 0, Encoding.UTF8.GetByteCount(header));

                        StreamUtils.CopyStream(data, requestStream, progressCallback);

                        requestStream.Write(Encoding.UTF8.GetBytes(footer), 0, Encoding.UTF8.GetByteCount(footer));

                        requestStream.Close();

                        request.BeginGetResponse(new AsyncCallback(ResponseCallback), rState);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("VKHttpRequestHelper.Upload failed to write data to request stream.", ex);
                        SafeClose(rState);
                        SafeInvokeCallback(rState.resultCallback, false, null);
                    }

                }, null);
#else

                var httpClient = new Windows.Web.Http.HttpClient();
                HttpMultipartFormDataContent content = new HttpMultipartFormDataContent();
                content.Add(new HttpStreamContent(data.AsInputStream()), paramName, fileName ?? "myDataFile");
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
#endif
            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.Upload failed.", exc);
#if SILVERLIGHT
                SafeClose(rState);
                   SafeInvokeCallback(rState.resultCallback, false, null);
#else
                SafeInvokeCallback(resultCallback, false, null);
#endif

            }
        }

#if SILVERLIGHT
        private static void ResponseCallback(IAsyncResult asynchronousResult)
        {
            var requestState = (RequestState)asynchronousResult.AsyncState;

            try
            {
                // State of request is asynchronous.

                var httpWebRequest = requestState.request;
                requestState.response = (HttpWebResponse)httpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                var responseStream = requestState.response.GetCompressedResponseStream();
                requestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the console.
                responseStream.BeginRead(requestState.BufferRead, 0, BUFFER_SIZE, ReadCallBack, requestState);
            }
            catch (WebException ex)
            {
                Logger.Error($"VKHttpRequestHelper.ResponseCallback failed. Got httpWebResponse = {ex.Response is HttpWebResponse} , uri = {requestState.request.RequestUri}", ex);

                var response = ex.Response as HttpWebResponse;
                if (response != null && ex.Response.ContentLength > 0)
                {
                    requestState.httpError = true;

                    requestState.response = response;

                    var responseStream = requestState.response.GetCompressedResponseStream();

                    requestState.streamResponse = responseStream;

                    // Begin the Reading of the contents of the HTML page and print it to the console.
                    responseStream.BeginRead(requestState.BufferRead, 0, BUFFER_SIZE, ReadCallBack, requestState);
                }
                else
                {
                    SafeClose(requestState);
                    SafeInvokeCallback(requestState.resultCallback, false, null);
                }
            }
            catch (Exception exc)
            {
                Logger.Error($"VKHttpRequestHelper.ResponseCallback failed. Uri ={requestState.request.RequestUri}", exc);
                SafeClose(requestState);
                SafeInvokeCallback(requestState.resultCallback, false, null);
            }
        }

        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            var requestState = (RequestState)asyncResult.AsyncState;

            try
            {
                var responseStream = requestState.streamResponse;
                var read = responseStream.EndRead(asyncResult);

                if (read > 0)
                {
                    requestState.readBytes.AddRange(requestState.BufferRead.Take(read));

                    var asynchronousResult = responseStream.BeginRead(requestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), requestState);
                }
                else
                {
                    var stringContent = Encoding.UTF8.GetString(requestState.readBytes.ToArray(), 0,
                                                                requestState.readBytes.Count);

                    SafeClose(requestState);

                    Logger.Info("<<<VKHttpRequestHelper completed http request, duration {0} ms. URI {1} ---->>>>> {2}",
                        (DateTime.Now - requestState.startTime).TotalMilliseconds,
                        requestState.request.RequestUri.OriginalString, stringContent);

                    SafeInvokeCallback(requestState.resultCallback, !requestState.httpError, stringContent);
                }
            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.ReadCallBack failed.", exc);
                SafeClose(requestState);
                SafeInvokeCallback(requestState.resultCallback, false, null);
            }
        }

        private static void SafeClose(RequestState state)
        {
            try
            {
                state.streamResponse?.Close();
                state.response?.Close();
            }
            catch (Exception exc)
            {
                Logger.Error("VKHttpRequestHelper.SafeClose failed.", exc);
            }
        }
#endif


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

            foreach (var kvp in parameters)
            {
                if (kvp.Key == null ||
                    kvp.Value == null)
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb = sb.Append("&");
                }

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
