using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Dinamik fiyatlandırma kuralı. CargoPricingService bu kurallara göre ücret hesaplar.
    /// Ücret; ağırlık, desi, şehir/bölge ve teslimat tipine göre belirlenir.
    /// </summary>
    public class CargoPrice : BaseEntity
    {
        public string Name { get; set; }
        public DeliveryType DeliveryType { get; set; }

        public decimal BasePrice { get; set; }        // Taban ücret
        public decimal PricePerKg { get; set; }       // Kilogram başına ücret
        public decimal PricePerDesi { get; set; }     // Desi başına ücret
        public decimal DifferentCityFee { get; set; } // Farklı şehir ek ücreti

        public bool IsActive { get; set; } = true;
    }
}
