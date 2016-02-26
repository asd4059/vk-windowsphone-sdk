using System.Collections.Generic;

namespace VK.WindowsPhone.SDK
{
    public class VKScope
    {
        public const string NOTIFY = "notify";
        public const string FRIENDS = "friends";
        public const string PHOTOS = "photos";
        public const string AUDIO = "audio";
        public const string VIDEO = "video";
        public const string DOCS = "docs";
        public const string NOTES = "notes";
        public const string PAGES = "pages";
        public const string STATUS = "status";
        public const string WALL = "wall";
        public const string GROUPS = "groups";
        public const string MESSAGES = "messages";
        public const string NOTIFICATIONS = "notifications";
        public const string STATS = "stats";
        public const string ADS = "ads";
        public const string OFFLINE = "offline";
        public const string NOHTTPS = "nohttps";
        public const string DIRECT = "direct";

        /// <summary>
        /// Converts integer permissions value into List of constants
        /// </summary>
        /// <param name="permissionsValue">Integer permissons value</param>
        /// <returns>List containing constant strings of permissions (scope)</returns>
        public static List<string> ParseVKPermissionsFromInteger(int permissionsValue)
        {
            var res = new List<string>();
            if ((permissionsValue & 1) > 0) res.Add(NOTIFY);
            if ((permissionsValue & 2) > 0) res.Add(FRIENDS);
            if ((permissionsValue & 4) > 0) res.Add(PHOTOS);
            if ((permissionsValue & 8) > 0) res.Add(AUDIO);
            if ((permissionsValue & 16) > 0) res.Add(VIDEO);
            if ((permissionsValue & 128) > 0) res.Add(PAGES);
            if ((permissionsValue & 1024) > 0) res.Add(STATUS);
            if ((permissionsValue & 2048) > 0) res.Add(NOTES);
            if ((permissionsValue & 4096) > 0) res.Add(MESSAGES);
            if ((permissionsValue & 8192) > 0) res.Add(WALL);
            if ((permissionsValue & 32768) > 0) res.Add(ADS);
            if ((permissionsValue & 65536) > 0) res.Add(OFFLINE);
            if ((permissionsValue & 131072) > 0) res.Add(DOCS);
            if ((permissionsValue & 262144) > 0) res.Add(GROUPS);
            if ((permissionsValue & 524288) > 0) res.Add(NOTIFICATIONS);
            if ((permissionsValue & 1048576) > 0) res.Add(STATS);
            return res;
        }
    }
}
