using System;

namespace YouRoomSharp
{
    internal static class DateTimeOffsetExtension
    {
        internal static string ToRfc3339DateFormat(this DateTimeOffset datetime)
        {
            return datetime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
        }
    }
}
