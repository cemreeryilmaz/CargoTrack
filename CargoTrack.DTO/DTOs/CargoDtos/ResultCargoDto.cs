using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.CargoDtos
{
    /// <summary>
    /// Listeleme ekranları için kargo özeti.
    /// </summary>
    public class ResultCargoDto
    {
        public Guid Id { get; set; }
        public string TrackCode { get; set; }

        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public double Weight { get; set; }
        public double Desi { get; set; }
        public decimal Price { get; set; }

        public CargoType CargoType { get; set; }
        public CargoStatus CargoStatus { get; set; }
        public DeliveryType DeliveryType { get; set; }

        // UI gösterim metinleri (Türkçe)
        public string CargoTypeName => CargoType.GetDisplayName();
        public string CargoStatusName => CargoStatus.GetDisplayName();
        public string DeliveryTypeName => DeliveryType.GetDisplayName();

        public string SenderFullName { get; set; }
        public string ReceiverFullName { get; set; }
        public string OriginBranchName { get; set; }
        public string DestinationBranchName { get; set; }

        public DateTime CreatedDate { get; set; }

        // Gecikmiş mi? (SLA süresi geçmiş ve teslim edilmemiş)
        public bool IsDelayed => EstimatedArrivalDate < DateTime.Now
                                 && CargoStatus != CargoStatus.Delivered
                                 && CargoStatus != CargoStatus.ReturnedToSender;
    }
}
