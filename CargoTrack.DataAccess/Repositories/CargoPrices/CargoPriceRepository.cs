using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.CargoPrices
{
    public class CargoPriceRepository(CargoTrackDbContext context)
        : GenericRepository<CargoPrice>(context), ICargoPriceRepository
    {
        public async Task<CargoPrice?> GetActiveByDeliveryTypeAsync(DeliveryType deliveryType)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.IsActive && p.DeliveryType == deliveryType);
        }
    }
}
