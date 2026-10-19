using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.DeliveryExceptionDtos
{
    /// <summary>
    /// Sorunlu kargo / başarısız teslimat kaydı girişi.
    /// </summary>
    public class CreateDeliveryExceptionDto
    {
        public Guid CargoId { get; set; }
        public DeliveryExceptionReason Reason { get; set; }
        public string? Description { get; set; }
    }
}
