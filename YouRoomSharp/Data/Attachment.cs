using System;

namespace YouRoomSharp.Data
{
    public enum AttachmentType
    {
        Image,
        Link,
        File
    }

    public class Attachment
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public AttachmentType AttachementType { get; set; }
        public Uri Url { get; set; }
    }
}
