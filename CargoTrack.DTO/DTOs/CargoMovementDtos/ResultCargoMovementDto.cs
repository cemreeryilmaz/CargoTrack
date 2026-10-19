using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.CargoMovementDtos
{
    public class ResultCargoMovementDto
    {
        public Guid Id { get; set; }
        public Guid CargoId { get; set; }

        public CargoStatus OldStatus { get; set; }
        public CargoStatus NewStatus { get; set; }
        public string OldStatusName => OldStatus.GetDisplayName();
        public string NewStatusName => NewStatus.GetDisplayName();

        public DateTime MovementDate { get; set; }
        public string? Description { get; set; }

        public string? LocationName { get; set; }   // Şube veya transfer merkezi adı
        public string? City { get; set; }           // Hareketin gerçekleştiği şehir (harita/rota için)
        public string? EmployeeName { get; set; }   // İşlemi yapan personel

        // Haritada gösterim için şehir adı (şehir yoksa konum adı)
        public string? DisplayLocation => City ?? LocationName;
    }
}
