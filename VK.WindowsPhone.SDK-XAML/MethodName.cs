using System;

namespace VK.WindowsPhone.SDK
{
    internal class MethodName : Attribute
    {
        public string Name;

        public MethodName(string name)
        {
            Name = name;
        }
    }
}
