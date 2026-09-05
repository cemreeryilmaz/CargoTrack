using CargoTrack.DTO.DTOs.BranchDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Business.Services.Branchs
{
    public interface IBranchService
    {
        Task<List<ResultBranchDto>> GetAllAsync();
        Task<UpdateBranchDto> GetByIdAsync(Guid id);
        Task CreateAsync(CreateBranchDto createBranchDto);
        Task UpdateAsync(UpdateBranchDto updateBranchDto);
        Task DeleteAsync(Guid id);
    }
}
