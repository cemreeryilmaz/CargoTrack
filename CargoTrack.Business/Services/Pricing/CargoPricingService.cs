using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.Pricing
{
    public class CargoPricingService(ICargoPriceRepository _cargoPriceRepository) : ICargoPricingService
    {
        /// <summary>
        /// Desi = En × Boy × Yükseklik / 3000
        /// </summary>
        public double CalculateDesi(double width, double height, double length)
        {
            var desi = (width * height * length) / 3000.0;
            return Math.Round(desi, 2);
        }

        public async Task<decimal> CalculatePriceAsync(DeliveryType deliveryType, double weight, double desi, bool sameCity)
        {
            var rule = await _cargoPriceRepository.GetActiveByDeliveryTypeAsync(deliveryType);

            // Aktif fiyat kuralı yoksa makul varsayılanlar kullanılır.
            decimal basePrice = rule?.BasePrice ?? deliveryType switch
            {
                DeliveryType.SameDay => 150m,
                DeliveryType.Express => 90m,
                _ => 50m
            };
            decimal pricePerKg = rule?.PricePerKg ?? 10m;
            decimal pricePerDesi = rule?.PricePerDesi ?? 8m;
            decimal differentCityFee = rule?.DifferentCityFee ?? 25m;

            // Ücretlendirmede ağırlık ve desiden büyük olan dikkate alınır (hacimsel ağırlık).
            decimal chargeableWeight = (decimal)weight;
            decimal chargeableDesi = (decimal)desi;

            decimal price = basePrice
                            + (pricePerKg * chargeableWeight)
                            + (pricePerDesi * chargeableDesi);

            if (!sameCity)
                price += differentCityFee;

            return Math.Round(price, 2);
        }

        public DateTime CalculateEstimatedArrival(DeliveryType deliveryType, DateTime shipmentDate, bool sameCity)
        {
            int baseDays = deliveryType switch
            {
                DeliveryType.SameDay => 0,
                DeliveryType.Express => 1,
                _ => 3
            };

            // Farklı şehirler arası gönderilerde +1 gün.
            if (!sameCity && deliveryType != DeliveryType.SameDay)
                baseDays += 1;

            var estimated = shipmentDate.AddDays(baseDays);

            // Aynı gün teslimatta günün sonunu hedefle.
            if (deliveryType == DeliveryType.SameDay)
                estimated = shipmentDate.Date.AddHours(18);

            return estimated;
        }
    }
}
