using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Teslimat hatası / sorunlu kargo kaydı. Gerekçe ve tarih bilgisiyle izlenir.
    /// Belirlenen başarısız deneme sayısı sonunda iade akışı başlatılır.
    /// </summary>
    public class DeliveryException : BaseEntity
    {
        public Guid CargoId { get; set; }

        public DeliveryExceptionReason Reason { get; set; }
        public string Description { get; set; }
        public DateTime ExceptionDate { get; set; }
        public int AttemptNumber { get; set; }

        // Hatayı kaydeden personel
        public Guid? EmployeeId { get; set; }

        // Navigation properties
        public virtual Cargo Cargo { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
