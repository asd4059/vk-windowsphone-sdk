using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Users
        {
            public static async Task<VKBackendResult<T>> Get<T>(string user_ids = null, string fields = null,
                string name_case = null)
                => await SendRequest<T>("users.get",Args(user_ids, fields, name_case));
        }
    }
}
