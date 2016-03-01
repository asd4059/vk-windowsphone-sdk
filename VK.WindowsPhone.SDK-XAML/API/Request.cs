using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        private static object[] Args(params object[] args) => args;

        private static async Task<VKBackendResult<T>> SendRequest<T>(IReadOnlyList<object> parameters = null, [CallerMemberName] string function = null)
        {
#if WINDOWS_UWP
            var methodInfo = typeof(Request).GetMethod(function);
#elif PCL
            var methodInfo = typeof(Request).GetTypeInfo().GetDeclaredMethod(function);
#endif

            var methodName = methodInfo.GetCustomAttribute<MethodName>()?.Name;
            var parametersInfo = methodInfo.GetParameters();

            var param = new Dictionary<string, string>();

            if (parameters != null && parametersInfo.Length == parameters.Count)
            {
                for (var i = 0; i < parametersInfo.Length; i++)
                    if (parameters[i] != parametersInfo[i].DefaultValue)
                    {
                        var attribute = parametersInfo[i].GetCustomAttribute<MemberName>();
                        var name = attribute != null ? attribute.Name : parametersInfo[i].Name;
                        param.Add(name, parameters[i].ToString());
                    }
            }

            var vkParam = new VKRequestParameters(methodName);
            if (param.Count > 0)
                vkParam.Parameters = param;

            return await new VKRequest(vkParam).DispatchAsync<T>();
        }
    }
}
