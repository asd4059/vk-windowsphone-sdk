﻿using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        public static class Audio
        {
            public static async Task<VKBackendResult<T>> Get<T>(long owner_id = -1, long album_id = -1,
                string audio_ids = null, int need_user = -1, int offset = 0, int count = 6000)
                => await SendRequest<T>("audio.get", owner_id, album_id, audio_ids, need_user, offset, count);

            public static async Task<VKBackendResult<T>> Search<T>(string q = null, int auto_complete = -1,
                int lyrics = -1, int performer_only = -1, int sort = -1, int search_own = 0, long offset = 0,
                int count = 30)
                =>
                    await
                        SendRequest<T>("audio.search", q, auto_complete, lyrics, performer_only, sort, search_own,
                            offset, count);

            public static async Task<VKBackendResult<T>> Delete<T>(long audio_id = -1, long owner_id = -1)
                => await SendRequest<T>("audio.delete", audio_id, owner_id);
        }
    }
}
