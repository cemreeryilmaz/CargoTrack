using System.ComponentModel.DataAnnotations;
using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.DTO.DTOs.TransferCenterDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.TransferCenters
{
    public class TransferCenterService(ITransferCenterRepository _repository) : ITransferCenterService
    {
        public async Task CreateAsync(CreateTransferCenterDto dto)
        {
            var entity = dto.Adapt<TransferCenter>();
            await _repository.CreateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                throw new ValidationException("Transfer merkezi bulunamadı.");

            await _repository.DeleteAsync(entity);
        }

        public async Task<List<ResultTransferCenterDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(t => new ResultTransferCenterDto
            {
                Id = t.Id,
                Name = t.Name,
                CityId = t.CityId,
                CityName = t.City?.Name,
                CreatedDate = t.CreatedDate
            }).ToList();
        }

        public async Task<UpdateTransferCenterDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                throw new ValidationException("Transfer merkezi bulunamadı.");

            return entity.Adapt<UpdateTransferCenterDto>();
        }

        public async Task UpdateAsync(UpdateTransferCenterDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity is null)
                throw new ValidationException("Transfer merkezi bulunamadı.");

            entity.Name = dto.Name;
            entity.CityId = dto.CityId;
            await _repository.UpdateAsync(entity);
        }
    }
}
