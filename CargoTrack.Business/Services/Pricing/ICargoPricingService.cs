using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.Pricing
{
    /// <summary>
    /// Dinamik fiyatlandırma ve SLA (tahmini teslimat) hesaplamaları.
    /// </summary>
    public interface ICargoPricingService
    {
        double CalculateDesi(double width, double height, double length);
        Task<decimal> CalculatePriceAsync(DeliveryType deliveryType, double weight, double desi, bool sameCity);
        DateTime CalculateEstimatedArrival(DeliveryType deliveryType, DateTime shipmentDate, bool sameCity);
    }
}
