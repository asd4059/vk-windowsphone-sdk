using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class NewsFeed
        {
            public static async Task<VKBackendResult<T>> Get<T>(string filters = null, int return_banned = -1,
                long start_time = -1, long end_time = -1, int max_photos = 5, string source_ids = null,
                string start_from = null, int count = 50, string fields = null)
                =>
                    await
                        SendRequest<T>("newsfeed.get", filters, return_banned, start_time, end_time, max_photos,
                            source_ids,
                            start_from,
                            count, fields);
        }
    }
}
