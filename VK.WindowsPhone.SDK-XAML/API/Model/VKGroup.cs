using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
   /// <summary>
   /// https://vk.com/dev/fields_groups
   /// </summary>
    public class VKGroup
    {
        public long id { get; set; }

        public string name { get; set; }

        public string screen_name { get; set; }

        public int is_closed { get; set; }

        public string deactivated { get; set; }

        public int is_admin { get; set; }

        public int admin_level { get; set; }

        public int is_member { get; set; }

        public string type { get; set; }

        public string photo_50 { get; set; }

        public string photo_100 { get; set; }

        public string photo_200 { get; set; }

        public long city { get; set; }

        public long country { get; set; }

        public Place place { get; set; }

        private string _desc = "";
        public string description
        {
            get { return _desc; }
            set { _desc = (value ?? "").ForUI(); }
        }

        public string wiki_page { get; set; }

        public int members_count { get; set; }

        public VKCounters counters { get; set; }

        public long start_date { get; set; }

        public long finish_date { get; set; }

        public int can_post { get; set; }

        public int can_see_all_posts { get; set; }

        public int can_upload_doc { get; set; }

        public int can_create_topic { get; set; }

        public string activity { get; set; }

        public string status { get; set; }

        public string contacts { get; set; }

        public string links { get; set; }

        public long fixed_post { get; set; }

        public int verified { get; set; }

        public string site { get; set; }

        public string GetMaxResolutionPhoto()
        {
            if (photo_200 != null)
                return photo_200;
            if (photo_100 != null)
                return photo_100;
            return photo_50;
        }

        public class Place
        {
            public long id { get; set; }

            public string title { get; set; }

            public int latitude { get; set; }

            public int longitude { get; set; }

            public string type { get; set; }

            public long country { get; set; }

            public long city { get; set; }

            public string address { get; set; }
        }
    }
}
