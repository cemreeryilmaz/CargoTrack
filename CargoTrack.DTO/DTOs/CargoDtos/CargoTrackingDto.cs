using CargoTrack.DTO.DTOs.CargoMovementDtos;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.CargoDtos
{
    /// <summary>
    /// Giriş gerektirmeyen public takip ekranı. Kişisel veriler gösterilmez.
    /// </summary>
    public class CargoTrackingDto
    {
        public string TrackCode { get; set; }

        public CargoStatus CargoStatus { get; set; }
        public string CargoStatusName => CargoStatus.GetDisplayName();

        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }

        public string OriginCityName { get; set; }
        public string DestinationCityName { get; set; }
        public string OriginBranchName { get; set; }
        public string DestinationBranchName { get; set; }

        // Hareket zaman çizelgesi (kişisel veri içermez)
        public List<ResultCargoMovementDto> Movements { get; set; } = new();
    }
}
