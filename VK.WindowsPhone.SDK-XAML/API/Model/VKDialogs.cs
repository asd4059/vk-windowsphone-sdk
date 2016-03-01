namespace VK.WindowsPhone.SDK.API.Model
{
    public class VKDialogs : VKList<VKDialog>
    {
        public int unread_dialogs { get; set; }
        public long real_offset { get; set; }
    }
}
