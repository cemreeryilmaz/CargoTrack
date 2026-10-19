using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.CargoMovements
{
    public class CargoMovementRepository(CargoTrackDbContext context)
        : GenericRepository<CargoMovement>(context), ICargoMovementRepository
    {
    }
}
