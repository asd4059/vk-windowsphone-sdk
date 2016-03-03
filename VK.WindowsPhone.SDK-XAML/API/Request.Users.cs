using System.Collections.Generic;
using System.Threading.Tasks;
using VK.WindowsPhone.SDK.Util;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Users
        {
            public static async Task<VKBackendResult<T>> Get<T>(string user_ids = null, string fields = null,
                string name_case = null)
                => await SendRequest<T>("users.get", user_ids, fields, name_case);
            //public static async Task<VKBackendResult<T>> Get<T>(IList<long> user_ids = null, IList<string> fields = null,
            //    string name_case = null)
            //    => await SendRequest<T>("users.get", user_ids?.GetCommaSeparated(), fields?.GetCommaSeparated(), name_case);
        }
    }
}
