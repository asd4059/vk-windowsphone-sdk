namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKDialogs : VKDialogs<VKDialog>
    {
    }

    public class VKDialogs<T> : VKList<T>
    {
        public int unread_dialogs { get; set; }
        public long real_offset { get; set; }
    }
}
