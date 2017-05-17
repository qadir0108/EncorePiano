using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WFP.ICT.Enum
{
    public class EnumHelper
    {
        public static IEnumerable<TextValuePair> GetEnumDescriptionValues(Type enumType)
        {
            var enums = new List<TextValuePair>();
            foreach (System.Enum item in System.Enum.GetValues(enumType))
            {
                var e = new TextValuePair()
                {
                    Text = GetDescriptions(item).FirstOrDefault(),
                    Value = Convert.ToInt32(item).ToString()
                };
                enums.Add(e);
            }
            return enums;
        }

        public static IEnumerable<TextValuePair> GetEnumTextValues(Type enumType)
        {
            var enums = new List<TextValuePair>();
            foreach (System.Enum item in System.Enum.GetValues(enumType))
            {
                var e = new TextValuePair()
                {
                    Text = System.Enum.GetName(enumType, item),
                Value = Convert.ToInt32(item).ToString()
                };
                enums.Add(e);
            }
            return enums;
        }

        public static IEnumerable<string> GetDescriptions(Type enumType)
        {
            var descs = new List<string>();
            foreach (var item in System.Enum.GetValues(enumType))
            {
                descs.AddRange(GetDescriptions(item as System.Enum));
            }
            return descs;
        }

        private static IEnumerable<string> GetDescriptions(System.Enum value)
        {
            var descs = new List<string>();
            var type = value.GetType();
            var name = System.Enum.GetName(type, value);
            var field = type.GetField(name);
            var fds = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
            foreach (DescriptionAttribute fd in fds)
            {
                descs.Add(fd.Description);
            }
            return descs;
        }
    }
}
