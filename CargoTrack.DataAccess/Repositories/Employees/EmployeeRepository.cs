using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.GenericRepositories;
using CargoTrack.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.Employees
{
    public class EmployeeRepository(CargoTrackDbContext context)
        : GenericRepository<Employee>(context), IEmployeeRepository
    {
        public async Task<Employee?> GetByAppUserIdAsync(Guid appUserId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.AppUserId == appUserId);
        }
    }
}
