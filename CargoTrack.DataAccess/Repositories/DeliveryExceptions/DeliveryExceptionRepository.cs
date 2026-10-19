using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.DeliveryExceptions
{
    public class DeliveryExceptionRepository(CargoTrackDbContext context)
        : GenericRepository<DeliveryException>(context), IDeliveryExceptionRepository
    {
    }
}
