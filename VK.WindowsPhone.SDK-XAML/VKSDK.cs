using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Security.Authentication.Web;
using VK.WindowsPhone.SDK.API;
using VK.WindowsPhone.SDK.Pages;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK
{
    public class VKSDK
    {
        public const string SDK_VERSION = "1.3.0";
        public const string API_VERSION = "5.45";

        private const string PLATFORM_ID = "winphone";


        private static VKSDK _instance;

        /// <summary>
        /// SDK instance
        /// </summary>
        internal static VKSDK Instance => _instance ?? (_instance = new VKSDK());

        /// <summary>
        /// Access token for API-requests
        /// </summary>
        protected VKAccessToken AccessToken;

        /// <summary>
        /// Default SDK access token key for storing value in IsolatedStorage.
        /// You shouldn't modify this key or value directly in IsolatedStorage.
        /// </summary>
        private const string VKSDK_ACCESS_TOKEN_ISOLATEDSTORAGE_KEY = "VKSDK_ACCESS_TOKEN_DONTTOUCH";

        internal const string VK_AUTH_STR_FRM = "https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}&display=mobile&v={3}&response_type=token&revoke={4}";

        /// <summary>
        /// Your VK app ID. 
        /// If you don't have one, create a standalone app here: https://vk.com/editapp?act=create 
        /// </summary>
        internal string CurrentAppID;

        internal static string DeviceId => Windows.System.UserProfile.AdvertisingManager.AdvertisingId;

        /// <summary>
        /// Initialize SDK
        /// </summary>
        /// <param name="appId">Your VK app ID. 
        /// If you don't have one, create a standalone app here: https://vk.com/editapp?act=create </param>
        public static void Initialize(string appId)
        {
            Logger = new DefaultLogger();

            if (string.IsNullOrEmpty(appId))
            {
                throw new Exception("VKSDK could not initialize. " +
                                    "Application ID cannot be null or empty. " +
                                    "If you don't have one, create a standalone app here: https://vk.com/editapp?act=create");
            }





            Instance.CurrentAppID = appId;
        }

        /// <summary>
        /// Initialize SDK with custom token key (e.g. saved from other source or for some test reasons)
        /// </summary>
        /// <param name="appId">Your VK app ID. 
        /// If you don't have one, create a standalone app here: https://vk.com/editapp?act=create </param>
        /// <param name="token">Custom-created access token</param>
        public static void Initialize(string appId, VKAccessToken token)
        {
            Initialize(appId);
            Instance.AccessToken = token;
            PerformTokenCheck(token, true);
        }

        public static Action<VKCaptchaUserRequest, Action<VKCaptchaUserResponse>> CaptchaRequest { private get; set; }

        //    public static Action<ValidationUserRequest, Action<ValidationUserResponse>> ValidationRequest { private get; set; }



        /// <summary>
        /// Invokes when existing token has expired
        /// </summary>
        public static event EventHandler<VKAccessTokenExpiredEventArgs> AccessTokenExpired = delegate { };

        /// <summary>
        /// Invokes when user authorization has been canceled
        /// </summary>
        public static event EventHandler<VKAccessDeniedEventArgs> AccessDenied = delegate { };

        /// <summary>
        /// Invokes when new access token has been received
        /// </summary>
        public static event EventHandler<VKAccessTokenReceivedEventArgs> AccessTokenReceived = delegate { };

        /// <summary>
        /// Invokes when predefined token has been received and accepted
        /// </summary>
        public static event EventHandler<VKAccessTokenAcceptedEventArgs> AccessTokenAccepted = delegate { };

        /// <summary>
        /// Invokes when access token has been renewed (e.g. user passed validation)
        /// </summary>
        public static event EventHandler<AccessTokenRenewedEventArgs> AccessTokenRenewed = delegate { };

        /// <summary>
        /// Invoked if current app installation is the installation from the VK mobile games catalog
        /// </summary>
        public static event EventHandler MobileCatalogInstallationDetected = delegate { };

        public static IVKLogger Logger;

        /// <summary>
        /// Starts authorization process. Opens and requests for access if VK App is installed. 
        /// Otherwise SDK will navigate current app to SDK navigation page and start OAuth in WebBrowser.
        /// </summary>
        /// <param name="scopeList">List of permissions for your app</param>
        /// <param name="revoke">If true user will be allowed to logout and change user</param>
        /// <param name="forceOAuth">SDK will use only OAuth authorization via WebBrowser</param>
        public static void Authorize(List<string> scopeList, bool revoke = false, bool forceOAuth = false, LoginType loginType = LoginType.WebView)
        {
            try
            {
                CheckConditions();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return;
            }

            if (scopeList == null)
                scopeList = new List<string>();

            // Force OFFLINE scope using
            if (!scopeList.Contains(VKScope.OFFLINE))
                scopeList.Add(VKScope.OFFLINE);


            switch (loginType)
            {
                case LoginType.VKApp:

                    AuthorizeVKApp(scopeList, revoke);
        
                    break;
                default:
                    AuthorizeWebAuthenticationBroker(scopeList, revoke);

                    break;
            }
        }

        private static void AuthorizeVKApp(List<string> scopeList, bool revoke)
        {
            VKAppLaunchAuthorizationHelper.AuthorizeVKApp("", Instance.CurrentAppID, scopeList, revoke);
        }

        public static async void AuthorizeWebAuthenticationBroker(List<string> scopeList, bool revoke)
        {
            const string REDIRECT_URL = "https://oauth.vk.com/blank.html";
            var uri = "https://oauth.vk.com/authorize?" + $"client_id={Instance.CurrentAppID}&" +
                      $"scope={scopeList.GetCommaSeparated()}&" + $"redirect_uri={REDIRECT_URL}&" + "display=mobile&" +
                      $"v={API_VERSION}&" + "response_type=token&" + $"revoke={(revoke ? 1 : 0)}";
            var authResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(uri), new Uri(REDIRECT_URL));
            var index = authResult.ResponseData.IndexOf('#');
            if (index > -1)
            {
                var result = authResult.ResponseData.Substring(index + 1);
                ProcessLoginResult(result, false, null);
            }
        }

        private enum CheckTokenResult
        {
            None,
            Success,
            Error
        };

        private static void CheckConditions()
        {
//#if SILVERLIGHT
//            if (Application.Current.RootVisual as Frame == null)
//                throw new Exception("Application.Current.RootVisual is supposed to be PhoneApplicationFrame");

//#endif
        }

//#if SILVERLIGHT
//        internal static PhoneApplicationFrame RootFrame => (PhoneApplicationFrame)Application.Current.RootVisual;
//#endif

        /// <summary>
        /// Check new access token and assign as instance token 
        /// </summary>
        /// <param name="tokenParams">Params of token</param>
        /// <param name="isTokenBeingRenewed">Flag indicating token renewal</param>
        /// <returns>Success if token has been assigned or error</returns>
        private static CheckTokenResult CheckAndSetToken(Dictionary<string, string> tokenParams, bool isTokenBeingRenewed)
        {
            var token = VKAccessToken.TokenFromParameters(tokenParams);
            if (token?.AccessToken == null)
            {
                return tokenParams.ContainsKey(VKAccessToken.SUCCESS) ? CheckTokenResult.Success : CheckTokenResult.Error;

                //var error = new VKError { error_code = (int)VKResultCode.UserAuthorizationFailed };
            }
            SetAccessToken(token, isTokenBeingRenewed);
            return CheckTokenResult.Success;
        }

        /// <summary>
        /// Save API access token in IsolatedStorage with default key.
        /// </summary>
        /// <param name="token">Access token to be used for API requests</param>
        /// <param name="renew">Is token being renewed. Raises different event handlers (AccessTokenReceived or AccessTokenRenewed)</param>
        public static void SetAccessToken(VKAccessToken token, bool renew = false)
        {
            if (Instance.AccessToken == null ||
                (Instance.AccessToken.AccessToken != token.AccessToken ||
                 Instance.AccessToken.ExpiresIn != token.ExpiresIn))
            {
                Instance.AccessToken = token;

                if (!renew)
                    AccessTokenReceived(null, new VKAccessTokenReceivedEventArgs { NewToken = token });
                else
                    AccessTokenRenewed(null, new AccessTokenRenewedEventArgs { Token = token });

                Instance.AccessToken.SaveTokenToIsolatedStorage(VKSDK_ACCESS_TOKEN_ISOLATEDSTORAGE_KEY);
            }
        }

        /// <summary>
        /// Get access token to be used for API requests.
        /// </summary>
        /// <returns>Received access token, null if user has not been authorized yet</returns>
        public static VKAccessToken GetAccessToken()
        {
            if (Instance.AccessToken != null)
            {
                if (Instance.AccessToken.IsExpired)
                    AccessTokenExpired(null, new VKAccessTokenExpiredEventArgs { ExpiredToken = Instance.AccessToken });

                return Instance.AccessToken;
            }

            return null;
        }

        /// <summary>
        /// Notify SDK that user denied login
        /// </summary>
        /// <param name="error">Description of error while authorizing user</param>
        public static void SetAccessTokenError(VKError error) => AccessDenied(null, new VKAccessDeniedEventArgs { AuthorizationError = error });

        private static bool PerformTokenCheck(VKAccessToken token, bool isUserDefinedToken = false)
        {
            if (token == null) return false;

            if (token.IsExpired)
            {
                AccessTokenExpired(null, new VKAccessTokenExpiredEventArgs { ExpiredToken = token });
                return false;
            }
            if (token.AccessToken != null)
            {
                if (isUserDefinedToken)
                    AccessTokenAccepted(null, new VKAccessTokenAcceptedEventArgs { Token = token });
                return true;
            }

            var error = new VKError { error_code = (int)VKResultCode.InvalidToken };
            AccessDenied(null, new VKAccessDeniedEventArgs { AuthorizationError = error });
            return false;
        }

        public static bool WakeUpSession()
        {
            var result = true;

            var token = VKAccessToken.TokenFromIsolatedStorage(VKSDK_ACCESS_TOKEN_ISOLATEDSTORAGE_KEY);

            if (!PerformTokenCheck(token))
                result = false;
            else
                Instance.AccessToken = token;

            TrackStats();

            return result;
        }

        private static void TrackStats()
        {
            long appId;
            var appIdParsed = long.TryParse(Instance.CurrentAppID, out appId);

            if (Instance.AccessToken != null)
            {
                var checkUserInstallRequest = new VKRequest("apps.checkUserInstall", "platform", PLATFORM_ID, "app_id", appId.ToString(), "device_id", DeviceId);

                checkUserInstallRequest.Dispatch<object>((res) =>
                    {
                        int responseVal;

                        if (res.Data != null && int.TryParse(res.Data.ToString(), out responseVal))
                        {
                            if (responseVal == 1)
                                MobileCatalogInstallationDetected?.Invoke(null, EventArgs.Empty);
                        }                        

                        var trackVisitorRequest = new VKRequest("stats.trackVisitor");

                        trackVisitorRequest.Dispatch(res2 => { }, jsonStr => new object());

                    });                
            }
            else
            {                
                if (!string.IsNullOrEmpty(DeviceId) && appIdParsed)
                {
                    var checkUserInstallRequest = new VKRequest("apps.checkUserInstall", "platform", PLATFORM_ID, "app_id", appId.ToString(), "device_id", DeviceId);

                    checkUserInstallRequest.Dispatch<object>(res =>
                    {
                        int responseVal;

                        if (res.Data != null && int.TryParse(res.Data.ToString(), out responseVal))
                        {
                            if (responseVal == 1)
                                MobileCatalogInstallationDetected?.Invoke(null, EventArgs.Empty);
                        }
                    });                
                }
            }

        }


        public static void CheckMobileCatalogInstallation(Action<bool> resultCallback)
        {
            VKRequest checkUserInstallRequest;
            long appId;
            var appIdParsed = long.TryParse(Instance.CurrentAppID, out appId);
            if (Instance.AccessToken != null)
                checkUserInstallRequest = new VKRequest("apps.checkUserInstall", "platform", PLATFORM_ID, "app_id", appId.ToString(), "device_id", DeviceId);
            else
            {
                if (!string.IsNullOrEmpty(DeviceId) && appIdParsed)
                    checkUserInstallRequest = new VKRequest("apps.checkUserInstall", "platform", PLATFORM_ID, "app_id", appId.ToString(), "device_id", DeviceId);
                else
                {
                    resultCallback(false);
                    return;
                }
            }

            checkUserInstallRequest.Dispatch<object>((res) =>
            {
                var responseVal = 0;

                if (res.Data != null)
                    int.TryParse(res.Data.ToString(), out responseVal);

                resultCallback(responseVal == 1);
            });                

        }

        /// <summary>
        /// Removes active token from memory and IsolatedStorage at default key.
        /// </summary>
        public static void Logout()
        {
            Instance.AccessToken = null;

            VKAccessToken.RemoveTokenInIsolatedStorage(VKSDK_ACCESS_TOKEN_ISOLATEDSTORAGE_KEY);

            VKUtil.ClearCookies();
        }

        public static bool IsLoggedIn => Instance.AccessToken != null && !Instance.AccessToken.IsExpired;


        internal static void ProcessLoginResult(string result, bool wasValidating, Action<VKValidationResponse> validationCallback)
        {
            var success = false;

            if (result == null)
                SetAccessTokenError(new VKError { error_code = (int)VKResultCode.UserAuthorizationFailed });
            else
            {
                var tokenParams = VKUtil.ExplodeQueryString(result);
                if (CheckAndSetToken(tokenParams, wasValidating) == CheckTokenResult.Success)
                {
                    success = true;

                    if (!wasValidating)
                        TrackStats();
                }
                else
                    SetAccessTokenError(new VKError { error_code = (int)VKResultCode.UserAuthorizationFailed });              
            }

            validationCallback?.Invoke(new VKValidationResponse { IsSucceeded = success });
        }

        internal static void InvokeCaptchaRequest(VKCaptchaUserRequest request, Action<VKCaptchaUserResponse> callback)
        {
            if (CaptchaRequest == null)
            {
                // no handlers are registered

                callback(
                    new VKCaptchaUserResponse()
                    {
                        Request = request,
                        EnteredString = string.Empty,
                        IsCancelled = true
                    });
            }
            else
            {
                VKExecute.ExecuteOnUIThread(() =>
                {
                    CaptchaRequest(request,
                                   callback);
                });
            }
        }

        internal static void InvokeValidationRequest(VKValidationRequest request, Action<VKValidationResponse> callback)
        {
            VKExecute.ExecuteOnUIThread(() =>
                {
                    var loginUserControl = new VKLoginUserControl
                    {
                        ValidationUri = request.ValidationUri,
                        ValidationCallback = callback
                    };


                    loginUserControl.ShowInPopup(Windows.UI.Xaml.Window.Current.Bounds.Width,
                         Windows.UI.Xaml.Window.Current.Bounds.Height); 

                });

        }
    }
}
