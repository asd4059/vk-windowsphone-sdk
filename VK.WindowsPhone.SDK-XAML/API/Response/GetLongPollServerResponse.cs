namespace VK.WindowsPhone.SDK_UWP.API.Response
{
    public class GetLongPollServerResponse
    {
        public string key { get; set; }
        public string server { get; set; }
        public long ts { get; set; }
        public long pts { get; set; }
    }
}
