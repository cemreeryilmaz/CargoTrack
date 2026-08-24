using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.Abouts
{
    public class AboutRepository : GenericRepository<About>, IAboutRepository
    {
        public AboutRepository(CargoTrackDbContext context) : base(context)
        {
        }
    }
}
