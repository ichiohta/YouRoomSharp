namespace YouRoomSharp.Data
{
    public class Participation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ParticipationGroup Group { get; set; }
    }
}
