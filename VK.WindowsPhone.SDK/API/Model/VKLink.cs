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
        public string image_src { get; set; }
    }
}
