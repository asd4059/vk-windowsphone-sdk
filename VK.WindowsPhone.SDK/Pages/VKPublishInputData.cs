using System.IO;

namespace VK.WindowsPhone.SDK.Pages
{    

    public class VKPublishInputData
    {

        public class VKLink
        {
            public string Uri { get; set; }

            public string Title { get; set; }

            public string Subtitle { get; set; }
        }


        public string Text { get; set; }

        public Stream Image { get; set; }

        public VKLink ExternalLink { get; set; }
    }
}
