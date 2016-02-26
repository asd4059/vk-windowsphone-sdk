using System;
using System.Windows.Navigation;
using VK.WindowsPhone.SDK;

namespace SDKSample81
{
    public class CustomUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            var handledAuthorization = VKUriMapperHandler.HandleUri(uri);

            if (handledAuthorization)
            {
                return new Uri("/MainPage.xaml", UriKind.Relative);
            }

            return uri;
        }
    }
}
