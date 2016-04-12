using System;
using System.Xml.Linq;

namespace YouRoomSharp.Serialization
{
    public static class XElementExtension
    {
        public static int? AsNullableInt(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                !string.IsNullOrEmpty(element.Value)
                ? (int?) int.Parse(element.Value)
                : null;
        }

        public static int AsInt(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));
            Assert.IsNotNull(element.Value, nameof(element.Value));
            return element.AsNullableInt().Value;
        }

        public static string AsNullableString(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));
            return element.Value;
        }

        public static string AsString(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));
            Assert.IsNotNull(element.Value, nameof(element.Value));
            return element.Value;
        }

        public static DateTimeOffset? AsNullableDateTimeOffset(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                !string.IsNullOrEmpty(element.Value)
                ? (DateTimeOffset?) DateTimeOffset.Parse(element.Value)
                : null;
        }

        public static DateTimeOffset AsDateTimeOffset(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));
            Assert.IsNotNull(element.Value, nameof(element.Value));
            return element.AsNullableDateTimeOffset().Value;
        }

        public static bool? AsNullableBool(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                !string.IsNullOrEmpty(element.Value)
                ? (bool?) bool.Parse(element.Value)
                : null;
        }

        public static bool AsBool(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));
            Assert.IsNotNull(element.Value, nameof(element.Value));
            return element.AsNullableBool().Value;
        }

        public static T? AsNullableEnum<T>(this XElement element) where T : struct
        {
            Assert.IsNotNull(element, nameof(element));

            return
                !string.IsNullOrEmpty(element.Value)
                ? (T?)Enum.Parse(typeof(T), element.Value, true)
                : null;
        }

        public static T AsEnum<T>(this XElement element) where T: struct
        {
            Assert.IsNotNull(element, nameof(element));
            Assert.IsNotNull(element.Value, nameof(element.Value));
            return element.AsNullableEnum<T>().Value;
        }

        public static Uri AsNullableUri(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));

            return
                !string.IsNullOrEmpty(element.Value)
                ? new Uri(element.Value)
                : null;
        }

        public static Uri AsUri(this XElement element)
        {
            Assert.IsNotNull(element, nameof(element));
            Assert.IsNotNull(element.Value, nameof(element.Value));
            return element.AsNullableUri();
        }
    }
}
