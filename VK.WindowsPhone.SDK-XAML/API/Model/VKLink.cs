using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKLink
    {
        public string url { get; set; }
        private string _title = "";
        public string title
        {
            get { return _title; }
            set { _title = (value ?? "").ForUI(); }
        }

        private string _desc = "";
        public string description
        {
            get { return _desc; }
            set { _desc = (value ?? "").ForUI(); }
        }

        public string caption { get; set; }
        public VKPhoto photo { get; set; }
        public bool is_external { get; set; }
        public VKProduct product { get; set; }
        public VKRating rating { get; set; }
        public VKApplication application { get; set; }
        public VKButton button { get; set; }
        public long preview_page { get; set; }
        public string preview_url { get; set; }
    }
}
