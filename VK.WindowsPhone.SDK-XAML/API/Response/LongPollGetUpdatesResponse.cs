using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK_UWP.API.Response
{
    public class LongPollGetUpdatesResponse
    {
        public long ts { get; set; }
        public long pts { get; set; }
        public List<List<object>> updates { get; set; } 
        public int failed { get; set; }

        public bool IsSuccessful => failed == 0;
    }
}
