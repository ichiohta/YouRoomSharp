using System;
using System.Xml.Linq;

namespace YouRoomSharp.Serialization
{
    internal static class XElementExtension
    {
        private static T? GetNullable<T>(this XElement element, string name, Func<string, T> parse) where T : struct
        {
            XElement child = element.Element(name);

            if (child == null || child.Attribute("nil")?.Value == "true")
                return null;

            return parse(child.Value);
        }

        internal static int GetInt(this XElement element, string name)
        {
            return int.Parse(element.Element(name).Value);
        }

        internal static int? GetNullableInt(this XElement element, string name)
        {
            return
                element.GetNullable(
                    name,
                    (value) => int.Parse(value));
        }

        internal static string GetString(this XElement element, string name)
        {
            return element.Element(name)?.Value;
        }

        internal static DateTimeOffset GetDateTimeOffset(this XElement element, string name)
        {
            return DateTimeOffset.Parse(element.Element(name).Value);
        }

        internal static DateTimeOffset? GetNullableDateTimeOffset(this XElement element, string name)
        {
            return
                element.GetNullable(
                    name,
                    (value) => DateTimeOffset.Parse(value));
        }

        internal static bool GetBool(this XElement element, string name)
        {
            return bool.Parse(element.Element(name).Value);
        }

        internal static bool? GetNullableBool(this XElement element, string name)
        {
            return
                element.GetNullable(
                    name,
                    (value) => bool.Parse(value));
        }

        internal static T GetEnum<T>(this XElement element, string name) where T : struct
        {
            return (T)Enum.Parse(typeof(T), element.Element(name).Value);
        }

        public static T? GetNullableEnum<T>(this XElement element, string name) where T : struct
        {
            return
                element.GetNullable(
                    name,
                    (value => (T)Enum.Parse(typeof(T), element.Element(name).Value)));
        }

        public static Uri GetUri(this XElement element, string name)
        {
            XElement value = element.Element(name);
            return value != null ? new Uri(element.Element(name).Value) : null;
        }
    }
}
