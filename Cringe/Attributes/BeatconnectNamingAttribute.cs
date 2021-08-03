using System;

namespace Cringe.Attributes
{
    public class BeatconnectNamingAttribute : Attribute
    {
        public string Name { get; }

        public BeatconnectNamingAttribute(string name)
        {
            Name = name;
        }

        public static string RetrieveNaming<T>(T from) where T : Enum
        {
            var enumType = typeof(T);
            var memberInfos = enumType.GetMember(from.ToString());
            var attributes = memberInfos[0].GetCustomAttributes(typeof(BeatconnectNamingAttribute), false);
            return (attributes.Length > 0) ? ((BeatconnectNamingAttribute) attributes[0]).Name : null;
        }
    }
}
