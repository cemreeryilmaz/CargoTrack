using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Şube personeli. Kargo hareketlerini ve teslimatları gerçekleştiren kişi.
    /// </summary>
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid BranchId { get; set; }

        // İlgili Identity kullanıcısı (opsiyonel)
        public Guid? AppUserId { get; set; }

        public string FullName => string.Join(" ", FirstName, LastName);

        // Navigation properties
        public virtual Branch Branch { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual IList<CargoMovement> CargoMovements { get; set; }
        public virtual IList<Delivery> Deliveries { get; set; }
        public virtual IList<DeliveryException> DeliveryExceptions { get; set; }
    }
}
