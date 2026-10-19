using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.DTO.DTOs.DeliveryDtos;
using CargoTrack.DTO.DTOs.DeliveryExceptionDtos;
using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.Cargos
{
    /// <summary>
    /// Kargo iş kuralları: durum makinesi, fiyat/SLA, teslimat kodu, iade akışı, takip.
    /// </summary>
    public interface ICargoService
    {
        Task CreateAsync(CreateCargoDto dto, Guid? userId = null, string? userName = null);
        Task UpdateAsync(UpdateCargoDto dto, Guid? userId = null, string? userName = null);
        Task DeleteAsync(Guid id, Guid? userId = null, string? userName = null);

        Task<UpdateCargoDto> GetByIdAsync(Guid id);
        Task<CargoDetailDto> GetDetailAsync(Guid id);

        // Rol bazlı listeleme (pagination + search + filter)
        Task<PagedResult<ResultCargoDto>> GetPagedAsync(CargoFilterDto filter);
        Task<PagedResult<ResultCargoDto>> GetBranchPagedAsync(Guid branchId, CargoFilterDto filter);
        Task<PagedResult<ResultCargoDto>> GetUserPagedAsync(Guid userId, CargoFilterDto filter);

        // Public takip (giriş gerektirmez, kişisel veri yok)
        Task<CargoTrackingDto?> GetTrackingAsync(string trackCode);

        // Public ana sayfa: son kargolar (kişisel veri içermez)
        Task<List<CargoTrackingDto>> GetRecentAsync(int count);

        // Operasyon
        Task ChangeStatusAsync(ChangeCargoStatusDto dto, Guid? employeeId = null, Guid? userId = null, string? userName = null);
        Task ConfirmDeliveryAsync(ConfirmDeliveryDto dto, Guid? employeeId = null, Guid? userId = null, string? userName = null);
        Task ReportDeliveryFailureAsync(CreateDeliveryExceptionDto dto, Guid? employeeId = null, Guid? userId = null, string? userName = null);

        // Müşteri teslimat tercihi (Adresteyim / Komşuya teslim / Randevu)
        Task SetDeliveryPreferenceAsync(Guid cargoId, Guid userId, DeliveryPreference preference,
            string? neighborAddress, DateTime? appointmentDate, string? userName = null);
    }
}
