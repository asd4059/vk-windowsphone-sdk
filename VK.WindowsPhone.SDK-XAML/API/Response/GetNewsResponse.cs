using System.Collections.Generic;
using VK.WindowsPhone.SDK.API.Model;

namespace VK.WindowsPhone.SDK.API.Response
{
    public class GetNewsResponse : GetNewsResponse<VKWallPost>
    {
    }

    public class GetNewsResponse<T> : VKList<T>
    {
        public List<VKUser> profiles { get; set; }
        public List<VKGroup> groups { get; set; }
        public string next_from { get; set; }
    }
}
