using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.CargoPriceDtos
{
    public class ResultCargoPriceDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public string DeliveryTypeName => DeliveryType.GetDisplayName();
        public decimal BasePrice { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal PricePerDesi { get; set; }
        public decimal DifferentCityFee { get; set; }
        public bool IsActive { get; set; }
    }
}
