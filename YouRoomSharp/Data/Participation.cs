using System;

namespace YouRoomSharp.Data
{
    public class Participation
    {
        public string Name { get; set; }
        public Group Group { get; set; }

        public int? Id { get; set; }
        public bool? Admin { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

    }
}
