using System.ComponentModel.DataAnnotations;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.DTO.DTOs.CargoPriceDtos;
using CargoTrack.Entity.Entities;
using Mapster;

namespace CargoTrack.Business.Services.CargoPrices
{
    public class CargoPriceService(ICargoPriceRepository _repository) : ICargoPriceService
    {
        public async Task CreateAsync(CreateCargoPriceDto dto)
        {
            var entity = dto.Adapt<CargoPrice>();
            await _repository.CreateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                throw new ValidationException("Fiyat kuralı bulunamadı.");

            await _repository.DeleteAsync(entity);
        }

        public async Task<List<ResultCargoPriceDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Adapt<List<ResultCargoPriceDto>>();
        }

        public async Task<UpdateCargoPriceDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                throw new ValidationException("Fiyat kuralı bulunamadı.");

            return entity.Adapt<UpdateCargoPriceDto>();
        }

        public async Task UpdateAsync(UpdateCargoPriceDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity is null)
                throw new ValidationException("Fiyat kuralı bulunamadı.");

            entity.Name = dto.Name;
            entity.DeliveryType = dto.DeliveryType;
            entity.BasePrice = dto.BasePrice;
            entity.PricePerKg = dto.PricePerKg;
            entity.PricePerDesi = dto.PricePerDesi;
            entity.DifferentCityFee = dto.DifferentCityFee;
            entity.IsActive = dto.IsActive;

            await _repository.UpdateAsync(entity);
        }
    }
}
