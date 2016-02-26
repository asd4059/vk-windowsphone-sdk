using System;
using System.Net;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK
{
    public static class VKUriMapperHandler
    {
        public static bool HandleUri(Uri uri)
        {        
            var tempUriStr = uri.ToString();

            if (tempUriStr.StartsWith("/Protocol"))
            {
                var outerQueryParamsString = VKUtil.GetParamsOfQueryString(tempUriStr);

                if (!string.IsNullOrEmpty(outerQueryParamsString))
                {
                    var outerQueryParams = VKUtil.ExplodeQueryString(outerQueryParamsString);
                    if (outerQueryParams.ContainsKey("encodedLaunchUri"))
                    {
                        var launchUriEncoded = outerQueryParams["encodedLaunchUri"];

                        var launchUriDecoded = WebUtility.UrlDecode(launchUriEncoded);

                        if (launchUriDecoded.StartsWith("vk") && launchUriDecoded.Contains("://authorize"))
                        {
                            launchUriDecoded = launchUriDecoded.Replace("authorize/#", "authorize/?");

                            var innerQueryParamsString = VKUtil.GetParamsOfQueryString(launchUriDecoded);

                            VKSDK.ProcessLoginResult(innerQueryParamsString, false, null);

                            return true;
                        }
                        else
                        {
                            // default start
                            return true;
                        }
                    }
                }
            }
            return false;
        }       
    }
}
