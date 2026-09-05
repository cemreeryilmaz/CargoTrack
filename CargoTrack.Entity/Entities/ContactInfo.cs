using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    public class ContactInfo : BaseEntity
    {
        public virtual string Address { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
    }
}
