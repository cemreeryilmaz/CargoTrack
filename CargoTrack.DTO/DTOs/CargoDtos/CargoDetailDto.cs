using CargoTrack.DTO.DTOs.CargoMovementDtos;
using CargoTrack.DTO.DTOs.DeliveryExceptionDtos;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.CargoDtos
{
    /// <summary>
    /// Kargo detay ekranı için tüm bilgiler (hareket geçmişi dahil).
    /// </summary>
    public class CargoDetailDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }

        public double Weight { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Desi { get; set; }
        public decimal Price { get; set; }

        public CargoType CargoType { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public DeliveryType DeliveryType { get; set; }

        public string CargoTypeName => CargoType.GetDisplayName();
        public string CargoStatusName => CargoStatus.GetDisplayName();
        public string DeliveryTypeName => DeliveryType.GetDisplayName();

        public string? DeliveryCode { get; set; }
        public string? DeliveredToName { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public int FailedAttemptCount { get; set; }

        // Müşteri teslimat tercihi
        public DeliveryPreference DeliveryPreference { get; set; }
        public string DeliveryPreferenceName => DeliveryPreference.GetDisplayName();
        public string? NeighborAddress { get; set; }
        public DateTime? AppointmentDate { get; set; }

        // Kargo dağıtıma çıktı mı (müşteri teslimat seçeneklerini görebilir mi)
        public bool IsOutForDelivery => CargoStatus == CargoStatus.OutForDelivery;

        public string SenderFullName { get; set; }
        public string ReceiverFullName { get; set; }

        public string OriginBranchName { get; set; }
        public string OriginCityName { get; set; }
        public string DestinationBranchName { get; set; }
        public string DestinationCityName { get; set; }

        public List<ResultCargoMovementDto> Movements { get; set; } = new();
        public List<ResultDeliveryExceptionDto> Exceptions { get; set; } = new();

        public bool IsDelayed => EstimatedArrivalDate < DateTime.Now
                                 && CargoStatus != CargoStatus.Delivered
                                 && CargoStatus != CargoStatus.ReturnedToSender;
    }
}
