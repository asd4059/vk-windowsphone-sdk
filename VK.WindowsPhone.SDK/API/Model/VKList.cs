using System.Collections.Generic;

namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKList<T>
    {
        public int count { get; set; }

        public List<T> items { get; set; }
    }
}
