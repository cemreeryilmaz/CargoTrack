using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Teslimat kaydı. Kargo dağıtıma çıktığında üretilen 6 haneli kod doğrulanarak teslim tamamlanır.
    /// </summary>
    public class Delivery : BaseEntity
    {
        public Guid CargoId { get; set; }

        public string DeliveryCode { get; set; }      // 6 haneli teslimat kodu
        public bool IsConfirmed { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string ReceivedByName { get; set; }     // Teslim alan kişi

        // Teslimatı yapan personel
        public Guid? EmployeeId { get; set; }

        // Navigation properties
        public virtual Cargo Cargo { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
