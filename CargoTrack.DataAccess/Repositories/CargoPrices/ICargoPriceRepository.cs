using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DataAccess.Repositories.CargoPrices
{
    public interface ICargoPriceRepository : IRepository<CargoPrice>
    {
        Task<CargoPrice?> GetActiveByDeliveryTypeAsync(DeliveryType deliveryType);
    }
}
