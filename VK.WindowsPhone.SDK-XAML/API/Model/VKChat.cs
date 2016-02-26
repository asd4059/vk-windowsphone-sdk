using System.Collections.Generic;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKChat
    {
        public long id { get; set; }
        public string type { get; set; }
        private string _title = "";
        public string title
        {
            get { return _title; }
            set { _title = (value ?? "").ForUI(); }
        }
        public long admin_id { get; set; }
        public List<long> users { get; set; }

        // TODO Удалить
        public string photo_100 { get; set; }
        // TODO Удалить
        public string photo_200 { get; set; }
    }
}
