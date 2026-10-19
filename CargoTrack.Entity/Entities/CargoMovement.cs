using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Kargo hareket kaydı. Her durum değişikliğinde oluşturulur ve asla silinmez.
    /// Eski/yeni durum, tarih, açıklama, şube/transfer merkezi ve işlemi yapan personel tutulur.
    /// </summary>
    public class CargoMovement : BaseEntity
    {
        public Guid CargoId { get; set; }

        public CargoStatus OldStatus { get; set; }
        public CargoStatus NewStatus { get; set; }

        public DateTime MovementDate { get; set; }
        public string Description { get; set; }

        // Hareketin gerçekleştiği şehir (rota/harita gösterimi için)
        public string? LocationCity { get; set; }

        // Hareketin gerçekleştiği konum (biri dolu olabilir)
        public Guid? BranchId { get; set; }
        public Guid? TransferCenterId { get; set; }

        // İşlemi yapan personel
        public Guid? EmployeeId { get; set; }

        // Navigation properties
        public virtual Cargo Cargo { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual TransferCenter TransferCenter { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
