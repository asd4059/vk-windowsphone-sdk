using System.Threading.Tasks;
using VK.WindowsPhone.SDK.API.Model;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Messages
        {
            public static async Task<VKBackendResult<VKDialogs>> GetDialogs(int offset = 0, int count = 20,
                long start_message_id = -1,
                int preview_length = 0, int unread = 0)
                => await SendRequest<VKDialogs>("messages.getDialogs",Args(offset, count, start_message_id, preview_length, unread));

            public static async Task<VKBackendResult<T>> Send<T>(long user_id = -1, string peer_id = null,
                string domain = null, long chat_id = -1, string user_ids = null, string message = null, long guid = -1,
                [MemberName("lat")] double latitude = 0, [MemberName("long")] double longitude = 0,
                string attachment = null, string forward_messages = null, long sticker_id = -1, int notification = -1)
                =>
                    await
                        SendRequest<T>("messages.send",Args(user_id, peer_id, domain, chat_id, user_ids, message, guid, latitude,
                            longitude, attachment, forward_messages, sticker_id, notification));
        }
    }
}
