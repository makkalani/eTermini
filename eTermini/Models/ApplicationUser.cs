using Microsoft.AspNetCore.Identity;

namespace eTermini.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
