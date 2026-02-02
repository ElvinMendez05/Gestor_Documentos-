using Microsoft.AspNetCore.Identity;

namespace Document_Manager.Domain.Entities
{
    public class Users : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
