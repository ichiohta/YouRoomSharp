using System;

namespace YouRoomSharp.Data
{
    public class Entry
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public int? ParentId { get; set; }
        public int RootId { get; set; }
        public int DesdescendantsCount { get; set; }
        public bool CanUpdate { get; set; }
        public bool HasRead { get; set; }
        public Participation Participation { get; set; }
        public Attachment Attachment { get; set; }
        public Entry[] Children { get; set; }

        public bool HasChildren
        {
            get
            {
                return Children != null && Children.Length > 0;
            }
        }
    }
}
