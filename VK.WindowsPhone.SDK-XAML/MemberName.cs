using System;

namespace VK.WindowsPhone.SDK
{
    internal class MemberName : Attribute
    {
        public string Name;

        public MemberName(string name)
        {
            Name = name;
        }
    }
}
