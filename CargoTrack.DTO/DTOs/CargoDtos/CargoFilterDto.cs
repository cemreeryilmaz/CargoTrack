using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.CargoDtos
{
    /// <summary>
    /// Kargo listeleme için arama / filtreleme / sayfalama parametreleri.
    /// </summary>
    public class CargoFilterDto
    {
        public string? Search { get; set; }          // TrackCode arama
        public CargoStatus? Status { get; set; }
        public DeliveryType? DeliveryType { get; set; }
        public Guid? BranchId { get; set; }
        public bool OnlyDelayed { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
