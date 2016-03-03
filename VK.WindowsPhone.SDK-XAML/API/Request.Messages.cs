using System.Threading.Tasks;
using VK.WindowsPhone.SDK_UWP.API.Response;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Messages
        {
            public static async Task<VKBackendResult<T>> GetDialogs<T>(int offset = 0, int count = 20,
                long start_message_id = -1,
                int preview_length = 0, int unread = 0)
                =>
                    await
                        SendRequest<T>("messages.getDialogs", offset, count, start_message_id, preview_length,
                            unread);

            public static async Task<VKBackendResult<T>> Send<T>(long user_id = -1, string peer_id = null,
                string domain = null, long chat_id = -1, string user_ids = null, string message = null, long guid = -1,
                [MemberName("lat")] double latitude = 0, [MemberName("long")] double longitude = 0,
                string attachment = null, string forward_messages = null, long sticker_id = -1, int notification = -1)
                =>
                    await
                        SendRequest<T>("messages.send", user_id, peer_id, domain, chat_id, user_ids, message, guid,
                            latitude,
                            longitude, attachment, forward_messages, sticker_id, notification);

            public static async Task<VKBackendResult<GetLongPollServerResponse>> GetLongPollServer(int use_ssl = 0,
                int need_pts = 0)
                => await SendRequest<GetLongPollServerResponse>("messages.getLongPollServer", use_ssl, need_pts);

            public static async Task<VKBackendResult<T>> GetLongPollHistory<T>(long ts = -1, long pts = -1,
                int preview_length = 0,
                int onlines = -1, string fields = null, int events_limit = 1000, int msgs_limit = 200,
                long max_msg_id = -1)
                =>
                    await
                        SendRequest<T>("messages.getLongPollHistory", ts, pts, preview_length, onlines, fields,
                            events_limit, msgs_limit, max_msg_id);
        }
    }
}
