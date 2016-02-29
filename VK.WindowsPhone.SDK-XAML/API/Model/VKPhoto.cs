using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKPhoto
    {
        public long id { get; set; }
        public long album_id { get; set; }
        public long owner_id { get; set; }
        public long user_id { get; set; }

        public string photo_75 { get; set; }
        public string photo_130 { get; set; }
        public string photo_604 { get; set; }
        public string photo_807 { get; set; }
        public string photo_1280 { get; set; }
        public string photo_2560 { get; set; }

        public int width { get; set; }
        public int height { get; set; }
        private string _text = "";
        public string text
        {
            get { return _text; }
            set { _text = (value ?? "").ForUI(); }
        }

        public int date { get; set; }

        public string GetMaxResolutionPhoto()
        {

            if (photo_2560 != null)
                return photo_2560;
            if (photo_1280 != null)
                return photo_1280;
            if (photo_807 != null)
                return photo_807;
            if (photo_604 != null)
                return photo_604;
            if (photo_130 != null)
                return photo_130;
            return photo_75;
        }
    }
}
