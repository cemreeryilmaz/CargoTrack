using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.CargoPriceDtos
{
    public class UpdateCargoPriceDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal PricePerDesi { get; set; }
        public decimal DifferentCityFee { get; set; }
        public bool IsActive { get; set; }
    }
}
