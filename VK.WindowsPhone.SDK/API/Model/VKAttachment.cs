namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKAttachment
    {
        public string type { get; set; }

        public VKAudio audio { get; set; }
        public VKVideo video { get; set; }
        public VKPhoto photo { get; set; }
        public VKPoll poll { get; set; }
        public VKDocument doc { get; set; }
        public VKLink link { get; set; }
        public VKWallPost wall { get; set; }
        public VKNote note { get; set; }
        public VKPage Page { get; set; }
      
    }
}
