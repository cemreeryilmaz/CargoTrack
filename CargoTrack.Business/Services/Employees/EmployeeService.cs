using System.ComponentModel.DataAnnotations;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DTO.DTOs.EmployeeDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.Employees
{
    public class EmployeeService(IEmployeeRepository _repository) : IEmployeeService
    {
        public async Task CreateAsync(CreateEmployeeDto dto)
        {
            var entity = dto.Adapt<Employee>();
            entity.IsActive = true;
            await _repository.CreateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                throw new ValidationException("Personel bulunamadı.");

            await _repository.DeleteAsync(entity);
        }

        public async Task<List<ResultEmployeeDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToResult).ToList();
        }

        public async Task<List<ResultEmployeeDto>> GetByBranchAsync(Guid branchId)
        {
            var list = await _repository.GetWhereAsync(e => e.BranchId == branchId);
            return list.Select(MapToResult).ToList();
        }

        public async Task<UpdateEmployeeDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                throw new ValidationException("Personel bulunamadı.");

            return entity.Adapt<UpdateEmployeeDto>();
        }

        public async Task<ResultEmployeeDto?> GetByAppUserIdAsync(Guid appUserId)
        {
            var entity = await _repository.GetByAppUserIdAsync(appUserId);
            return entity is null ? null : MapToResult(entity);
        }

        public async Task UpdateAsync(UpdateEmployeeDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity is null)
                throw new ValidationException("Personel bulunamadı.");

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Title = dto.Title;
            entity.IsActive = dto.IsActive;
            entity.BranchId = dto.BranchId;
            entity.AppUserId = dto.AppUserId;

            await _repository.UpdateAsync(entity);
        }

        private static ResultEmployeeDto MapToResult(Employee e) => new()
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Title = e.Title,
            IsActive = e.IsActive,
            BranchId = e.BranchId,
            BranchName = e.Branch?.Name,
            CreatedDate = e.CreatedDate
        };
    }
}
