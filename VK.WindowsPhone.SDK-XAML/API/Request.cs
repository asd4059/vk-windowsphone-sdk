using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.API
{
    public static partial class Request
    {
        private static async Task<VKBackendResult<T>> SendRequest<T>(string fullFunctionName, params object[] parameters)
        {
            var splitStr = fullFunctionName.Split('.');
            var className = splitStr[0];
            var functionName = splitStr[1];

            var caller = typeof(Request).GetTypeInfo().DeclaredNestedTypes.First(t => string.Equals(t.Name, className, StringComparison.CurrentCultureIgnoreCase));
            var methodInfo = caller.DeclaredMethods.First(m=>string.Equals(m.Name, functionName, StringComparison.CurrentCultureIgnoreCase));

            var parametersInfo = methodInfo.GetParameters();

            var param = new Dictionary<string, string>();

            if (parameters != null && parametersInfo.Length == parameters.Length)
                for (var i = 0; i < parametersInfo.Length; i++)
                    if (parameters[i]!=null && !parameters[i].Equals(parametersInfo[i].DefaultValue))
                    {
                        var attribute = parametersInfo[i].GetCustomAttribute<MemberName>();
                        var name = attribute != null ? attribute.Name : parametersInfo[i].Name;
                        param.Add(name, parameters[i].ToString());
                    }

            var vkParam = new VKRequestParameters(fullFunctionName);
            if (param.Count > 0)
                vkParam.Parameters = param;

            return await new VKRequest(vkParam).DispatchAsync<T>();
        }
    }
}
