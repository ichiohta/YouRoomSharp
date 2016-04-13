using System;

namespace YouRoomSharp.Data
{
    public class Group
    {
        public string Name { get; set; }
        public int ToParam { get; set; }
        public int? Id { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool? Opened { get; set; }
        public bool? IsExpired { get; set; }
    }
}
