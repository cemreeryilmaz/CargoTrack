using CargoTrack.DTO.DTOs.EmployeeDtos;

namespace CargoTrack.Business.Services.Employees
{
    public interface IEmployeeService
    {
        Task<List<ResultEmployeeDto>> GetAllAsync();
        Task<List<ResultEmployeeDto>> GetByBranchAsync(Guid branchId);
        Task<UpdateEmployeeDto> GetByIdAsync(Guid id);
        Task<ResultEmployeeDto?> GetByAppUserIdAsync(Guid appUserId);
        Task CreateAsync(CreateEmployeeDto dto);
        Task UpdateAsync(UpdateEmployeeDto dto);
        Task DeleteAsync(Guid id);
    }
}
