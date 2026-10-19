using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities;

public class Cargo : BaseEntity
{
    public string TrackCode { get; set; }           // MYC-2026-849251 formatında benzersiz takip no
    public DateTime ShipmentDate { get; set; }
    public DateTime EstimatedArrivalDate { get; set; } // SLA - otomatik hesaplanan tahmini teslim tarihi

    // Ölçüler (desi hesabı için - cm)
    public double Weight { get; set; }              // kg
    public double Width { get; set; }               // En
    public double Height { get; set; }              // Yükseklik
    public double Length { get; set; }              // Boy
    public double Desi { get; set; }                // En × Boy × Yükseklik / 3000

    // Fiyatlandırma
    public decimal Price { get; set; }

    // Teslimat
    public string? DeliveryCode { get; set; }       // 6 haneli kod (dağıtıma çıkınca üretilir)
    public string? DeliveredToName { get; set; }    // Teslim alan kişi
    public DateTime? DeliveredDate { get; set; }
    public int FailedAttemptCount { get; set; }     // Başarısız teslim deneme sayısı

    // Müşteri teslimat tercihi (kargo dağıtıma çıkınca seçilir)
    public DeliveryPreference DeliveryPreference { get; set; } = DeliveryPreference.None;
    public string? NeighborAddress { get; set; }    // "Komşuma teslim et" seçilince komşu adresi
    public DateTime? AppointmentDate { get; set; }  // "Başka bir güne randevu" seçilince yeni tarih

    // İlişkiler
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid OriginBranchId { get; set; }
    public Guid DestinationBranchId { get; set; }

    // Kargonun bulunduğu güncel konum (opsiyonel)
    public Guid? CurrentBranchId { get; set; }
    public Guid? CurrentTransferCenterId { get; set; }

    public CargoType CargoType { get; set; }
    public CargoStatus CargoStatus { get; set; }
    public DeliveryType DeliveryType { get; set; }

    // Navigation properties
    public virtual AppUser Sender { get; set; }
    public virtual AppUser Receiver { get; set; }
    public virtual Branch OriginBranch { get; set; }
    public virtual Branch DestinationBranch { get; set; }
    public virtual Branch CurrentBranch { get; set; }
    public virtual TransferCenter CurrentTransferCenter { get; set; }

    public virtual IList<CargoMovement> CargoMovements { get; set; }
    public virtual IList<DeliveryException> DeliveryExceptions { get; set; }
    public virtual Delivery Delivery { get; set; }
}
