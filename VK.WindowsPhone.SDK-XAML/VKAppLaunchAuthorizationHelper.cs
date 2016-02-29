using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using VK.WindowsPhone.SDK.Util;
using Windows.System;
using System.IO;

namespace VK.WindowsPhone.SDK
{
    public static class VKAppLaunchAuthorizationHelper
    {
        private const string _launchUriStrFrm = @"vkappconnect://authorize?State={0}&ClientId={1}&Scope={2}&Revoke={3}&RedirectUri={4}";

        public static async Task AuthorizeVKApp(
            string state,
            string clientId, 
            IList<string> scopeList,
            bool revoke)
        {
            var redirectUri = await GetRedirectUri();

            var uriString = string.Format(_launchUriStrFrm,
                WebUtility.UrlEncode(state ?? string.Empty),
                clientId,
                scopeList.GetCommaSeparated(),
                revoke,
                redirectUri);

            var fallbackUri = string.Format(VKSDK.VK_AUTH_STR_FRM,
                VKSDK.Instance.CurrentAppID,
               scopeList.GetCommaSeparated(),
               WebUtility.UrlEncode("vk" + clientId + "://authorize" ),
               VKSDK.API_VERSION, 
               revoke ? 1 : 0);

            try
            {
                await Launcher.LaunchUriAsync(new Uri(uriString), new LauncherOptions() { FallbackUri = new Uri(fallbackUri) });
            }
            catch (Exception)
            {
      
            }
        }

        private static async Task<string> GetRedirectUri() => await GetVKLoginCallbackSchemeName() + "://authorize";

        private static async Task<string> GetVKLoginCallbackSchemeName()
        {
            var result = await GetFilteredManifestAppAttributeValue("Protocol", "Name", "vk");
            return result;
        }

        internal static async Task<string> GetFilteredManifestAppAttributeValue(string node, string attribute, string prefix)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VKConfig.xml"));
            using (var strm = await file.OpenStreamForReadAsync())
            {
                var xml = XElement.Load(strm);
                var filteredAttributeValue = (from app in xml.Descendants(node)
                                              let xAttribute = app.Attribute(attribute)
                                              where xAttribute != null
                                              select xAttribute.Value).FirstOrDefault(a => a.StartsWith(prefix));

                return string.IsNullOrWhiteSpace(filteredAttributeValue) ? string.Empty : filteredAttributeValue;
            }
        }
    }
}
