using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Friends
        {
            [MethodName("friends.get")]
            public static async Task<VKBackendResult<T>> Get<T>(long user_id = -1, string order = null,
                long list_id = -1, int count = -1, int offset = -1, string fields = null, string name_case = null)
                => await SendRequest<T>(Args(user_id, order, list_id, count, offset, fields, name_case));
        }
    }
}
