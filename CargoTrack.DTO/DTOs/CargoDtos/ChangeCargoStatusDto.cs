using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.CargoDtos
{
    /// <summary>
    /// Kargo durum değişikliği (yeni hareket girişi) için.
    /// </summary>
    public class ChangeCargoStatusDto
    {
        public Guid CargoId { get; set; }
        public CargoStatus NewStatus { get; set; }
        public string? Description { get; set; }

        public Guid? BranchId { get; set; }
        public Guid? TransferCenterId { get; set; }
    }
}
