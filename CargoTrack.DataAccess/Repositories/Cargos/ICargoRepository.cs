using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Cargos
{
    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<Cargo?> GetByTrackCodeAsync(string trackCode);
        Task<Cargo?> GetWithDetailsAsync(Guid id);
    }
}
