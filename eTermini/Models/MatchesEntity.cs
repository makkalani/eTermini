namespace eTermini.Models
{
    public class MatchesEntity
    {
        public int ID { get; set; }
        public string? OrganizerID { get; set; }
        public DateTime Date { get; set; }
        public int LocationID { get; set; }
        public int PlayerCount { get; set; }

        public ApplicationUser? Organizer { get; set; }
        public string LocationName { get; set; }

        public virtual ICollection<JoinRequest> JoinRequests { get; set; }
    }
}
