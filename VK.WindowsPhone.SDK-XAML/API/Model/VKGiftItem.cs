namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKGiftItem
    {
        public long id { get; set; }
        public long from_id { get; set; }
        public string message { get; set; }
        public long date { get; set; }
        public VKGift gift { get; set; }
        public int privacy { get; set; }
    }
}
