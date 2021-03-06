﻿namespace VK.WindowsPhone.SDK.API
{
    public class VKBackendResult<T>
    {
        public VKResultCode ResultCode { get; set; }

        public string ResultString { get; set; }

        public T Data { get; set; }

        public VKError Error { get; set; }

        public bool IsSuccessful => ResultCode == VKResultCode.Succeeded;
    }
}
