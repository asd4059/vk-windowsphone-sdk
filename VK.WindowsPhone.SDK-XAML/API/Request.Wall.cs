using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Wall
        {
            public static async Task<VKBackendResult<T>> Get<T>(string owner_id = null, string domain = null,
                long offset = -1, int count = -1, string filter = null, int extended = 0, string fields = null)
                => await SendRequest<T>("wall.get", owner_id, domain, offset, count, filter, extended, fields);
        }
    }
}
