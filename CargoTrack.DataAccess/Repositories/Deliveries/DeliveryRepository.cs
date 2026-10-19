using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Deliveries
{
    public class DeliveryRepository(CargoTrackDbContext context)
        : GenericRepository<Delivery>(context), IDeliveryRepository
    {
        public async Task<Delivery?> GetByCargoIdAsync(Guid cargoId)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.CargoId == cargoId);
        }
    }
}
