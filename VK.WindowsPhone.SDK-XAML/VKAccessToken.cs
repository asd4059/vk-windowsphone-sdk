﻿using System.Collections.Generic;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK
{
    public class VKAccessToken
    {
        public const string ACCESS_TOKEN = "access_token";
        public const string EXPIRES_IN = "expires_in";
        public const string USER_ID = "user_id";
        public const string SECRET = "secret";
        public const string HTTPS_REQUIRED = "https_required";
        public const string CREATED = "created";
        public const string SUCCESS = "success";

        /// <summary>
        /// String token for use in request parameters
        /// </summary>
        public string AccessToken;

        /// <summary>
        /// Time when token expires
        /// </summary>
        public int ExpiresIn;

        /// <summary>
        /// Current user id for this token
        /// </summary>
        public string UserId;

        /// <summary>
        /// User secret to sign requests (if nohttps used)
        /// </summary>
        public string Secret;

        /// <summary>
        /// If user sets "Always use HTTPS" setting in his profile, it will be true
        /// </summary>
        public bool IsHttpsRequired;

        /// <summary>
        /// Indicates time of token creation
        /// </summary>
        public long Created;

        /// <summary>
        /// Save token into Isolated Storage with key
        /// </summary>
        /// <param name="tokenKey">Your key for saving settings</param>
        public void SaveTokenToIsolatedStorage(string tokenKey) => Windows.Storage.ApplicationData.Current.LocalSettings.Values[tokenKey] = SerializeTokenData();

        /// <summary>
        /// Removes token from Isolated Storage with specified key
        /// </summary>
        /// <param name="tokenKey">Your key for saving settings</param>
        public static void RemoveTokenInIsolatedStorage(string tokenKey) => Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(tokenKey);

        /// <summary>
        /// Serialize token into string
        /// </summary>
        /// <returns></returns>
        protected string SerializeTokenData()
        {
            var args = new Dictionary<string, object>
            {
                {ACCESS_TOKEN, AccessToken},
                {EXPIRES_IN, ExpiresIn},
                {USER_ID, UserId},
                {CREATED, Created}
            };

            if (Secret != null)
                args.Add(SECRET, Secret);

            if (IsHttpsRequired)
                args.Add(HTTPS_REQUIRED, "1");

            return VKUtil.JoinParams(args);
        }

        /// <summary>
        /// Retreive token from key-value query string
        /// </summary>
        /// <param name="urlString">String that contains URL-query part with token. E.g. access_token=eee&expires_in=0..</param>
        /// <returns>parsed token</returns>
        public static VKAccessToken FromUrlString(string urlString)
        {
            if (urlString == null)
                return null;

            var args = VKUtil.ExplodeQueryString(urlString);

            return TokenFromParameters(args);
        }

        /// <summary>
        /// Retreive token from key-value map
        /// </summary>
        /// <param name="args">Dictionary containing token info</param>
        /// <returns>Parsed token</returns>
        public static VKAccessToken TokenFromParameters(Dictionary<string, string> args)
        {
            if (args == null || args.Count == 0)
                return null;

            try
            {
                var token = new VKAccessToken();
                args.TryGetValue(ACCESS_TOKEN, out token.AccessToken);

                string expiresValue;
                args.TryGetValue(EXPIRES_IN, out expiresValue);
                int.TryParse(expiresValue, out token.ExpiresIn);

                args.TryGetValue(USER_ID, out token.UserId);
                args.TryGetValue(SECRET, out token.Secret);

                if (args.ContainsKey(HTTPS_REQUIRED))
                    token.IsHttpsRequired = args[HTTPS_REQUIRED] == "1";
                else if (token.Secret == null)
                    token.IsHttpsRequired = true;

                if (args.ContainsKey(CREATED))
                    long.TryParse(args[CREATED], out token.Created);
                else
                    token.Created = VKUtil.CurrentTimeMillis();

                return token;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Retreives token from Isolated Storage
        /// </summary>
        /// <param name="tokenKey">Your key for saving settings</param>
        /// <returns>Previously saved token or null</returns>
        public static VKAccessToken TokenFromIsolatedStorage(string tokenKey)
        {
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(tokenKey))
                return null;

            var tokenString = Windows.Storage.ApplicationData.Current.LocalSettings.Values[tokenKey].ToString();

            return FromUrlString(tokenString);
        }

        /// <summary>
        /// Retreive token from file. Token must be saved into file via VKAccessToken.SaveToTokenFile()
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static VKAccessToken TokenFromFile(string filename)
        {
            try
            {
                var data = VKUtil.FileToString(filename);
                return FromUrlString(data);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks token expiration time
        /// </summary>
        /// <returns>true if token has expired</returns>
        public bool IsExpired => ExpiresIn > 0 && ExpiresIn * 1000 + Created < VKUtil.CurrentTimeMillis();
    }
}
