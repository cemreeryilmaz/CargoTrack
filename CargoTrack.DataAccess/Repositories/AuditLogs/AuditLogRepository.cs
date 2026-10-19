using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;

namespace CargoTrack.DataAccess.Repositories.AuditLogs
{
    public class AuditLogRepository(CargoTrackDbContext context)
        : GenericRepository<AuditLog>(context), IAuditLogRepository
    {
    }
}
