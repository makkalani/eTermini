using Microsoft.AspNetCore.Identity;

namespace eTermini.Models
{
    public class JoinRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; } // User who requests to join
        public int MatchId { get; set; } // Match being requested to join
        public string Status { get; set; } // Pending/Accepted/Rejected
        public virtual IdentityUser User { get; set; }
        public virtual MatchesEntity Match { get; set; }
    }
}
