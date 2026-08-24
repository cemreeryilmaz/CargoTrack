using CargoTrack.DataAccess.Repositories.Branchs;
using CargoTrack.DTO.DTOs.BranchDtos;
using CargoTrack.Entity.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Business.Services.Branchs
{
    public class BranchService(IBranchRepository _branchRepository) : IBranchService
    {
        public async Task CreateAsync(CreateBranchDto createBranchDto)
        {
            var branch = createBranchDto.Adapt<Branch>();
            await _branchRepository.CreateAsync(branch);
        }

        public async Task DeleteAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch is null)
            {
                throw new ValidationException("Branch not found.");
            }
            await _branchRepository.DeleteAsync(branch);
        }

        public async Task<List<ResultBranchDto>> GetAllAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            return branches.Adapt<List<ResultBranchDto>>().ToList();
        }

        public async Task<UpdateBranchDto> GetByIdAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch is null)
            {
                throw new ValidationException("Branch not found.");
            }
            return branch.Adapt<UpdateBranchDto>();
        }

        public async Task UpdateAsync(UpdateBranchDto updateBranchDto)
        {
            var branch = updateBranchDto.Adapt<Branch>();
            await _branchRepository.UpdateAsync(branch);
        }
    }
}