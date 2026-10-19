using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Cargos
{
    public class CargoRepository(CargoTrackDbContext context) : GenericRepository<Cargo>(context), ICargoRepository
    {
        public async Task<Cargo?> GetByTrackCodeAsync(string trackCode)
        {
            return await _dbSet
                .Include(c => c.OriginBranch).ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch).ThenInclude(b => b.City)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Include(c => c.CargoMovements)
                .FirstOrDefaultAsync(c => c.TrackCode == trackCode);
        }

        public async Task<Cargo?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.OriginBranch).ThenInclude(b => b.City)
                .Include(c => c.DestinationBranch).ThenInclude(b => b.City)
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .Include(c => c.CargoMovements)
                .Include(c => c.DeliveryExceptions)
                .Include(c => c.Delivery)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
