using Newtonsoft.Json;

namespace VK.WindowsPhone.SDK.API.Model
{
    public abstract class VKApiModelBase
    {

        public abstract void DeserializeFromJSON(JsonToken jsonToken);
    }
}
