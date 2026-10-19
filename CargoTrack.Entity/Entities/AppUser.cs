using Microsoft.AspNetCore.Identity;

namespace CargoTrack.Entity.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Manager / Employee kullanıcıları için bağlı olduğu şube (rol bazlı veri filtreleme)
        public Guid? BranchId { get; set; }

        public string FullName => string.Join(" ", FirstName, LastName);

        // Navigation properties
        public virtual Branch Branch { get; set; }
        public virtual IList<Cargo> SentCargos { get; set; }
        public virtual IList<Cargo> ReceivedCargos { get; set; }
        public virtual IList<Address> Addresses { get; set; }
    }
}
