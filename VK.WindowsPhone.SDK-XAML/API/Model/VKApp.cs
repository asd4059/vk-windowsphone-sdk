namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKApp
    {
        public enum Type
        {
            app,
            game,
            site,
            standalone
        }
        public ulong id { get; set; }
        public string title { get; set; }
        public string screen_name { get; set; }
        public string description { get; set; }
        public string icon_100 { get; set; }
        public string icon_200 { get; set; }
        public string icon_75 { get; set; }
        public string icon_50 { get; set; }
        public string icon_16 { get; set; }
        public string banner_186 { get; set; }
        public string banner_896 { get; set; }
        public Type type { get; set; }
        public string section { get; set; }
        public string author_url { get; set; }
        public ulong author_id { get; set; }
        public ulong author_group { get; set; }
        public ulong members_count { get; set; }
        public long published_date { get; set; }
        public ulong catalog_position { get; set; }
        public VKPhoto screenshots { get; set; }
        public bool international { get; set; }
        //	тип турнирной таблицы (0 - не поддерживается, 1 - по уровню, 2 - по очкам).
        public int leaderboard_type { get; set; }
        public int genre_id { get; set; }
        public string genre { get; set; }
        public string platform_id { get; set; }
        public bool is_in_catalog { get; set; }

    }
}
