using System;

namespace VK.WindowsPhone.SDK.Util
{
    public class VKExecute
    {
        public static void ExecuteOnUIThread(Action action)
        {
            var d = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            if (d.HasThreadAccess)
            {
                action();
            }
            else
            {
                d.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        action();
                    });
            }
        }
    }
}
