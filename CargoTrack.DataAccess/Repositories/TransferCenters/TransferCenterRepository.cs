using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.TransferCenters
{
    public class TransferCenterRepository(CargoTrackDbContext context)
        : GenericRepository<TransferCenter>(context), ITransferCenterRepository
    {
    }
}
