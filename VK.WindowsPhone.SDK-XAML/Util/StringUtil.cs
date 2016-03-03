using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace VK.WindowsPhone.SDK.Util
{
    public static class StrUtil
    {
        public static string ForUI(this string backendTextString)
        {
            if (string.IsNullOrEmpty(backendTextString))
                return string.Empty;

            var res = backendTextString;

            res = res.Replace("\r\n", "\n");

            res = res.Replace("\n", "\r\n");

            res = WebUtility.HtmlDecode(res);

            return res.Trim();
        }

        public static string MakeIntoOneLine(this string str)
        {
            if (str == null)
                return string.Empty;

            str = str.Replace(Environment.NewLine, " ");
            str = str.Replace("\n", " ");
            return str;
        }

        public static string GetCommaSeparated(this IList<string> ids)
        {
            var sb = new StringBuilder();

            var count = ids.Count;

            for (var i = 0; i < count; i++)
            {
                sb = sb.Append(ids[i]);
                if (i != count - 1)
                    sb = sb.Append(",");
            }

            return sb.ToString();
        }

        public static string GetCommaSeparated(this IList<long> ids)
        {
            var sb = new StringBuilder();

            var count = ids.Count;

            for (var i = 0; i < count; i++)
            {
                sb = sb.Append(ids[i]);
                if (i != count - 1)
                    sb = sb.Append(",");
            }

            return sb.ToString();
        }
    }
}
