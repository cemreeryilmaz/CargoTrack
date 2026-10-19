using CargoTrack.DTO.DTOs.AuditLogDtos;
using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.AuditLogs
{
    public interface IAuditLogService
    {
        Task LogAsync(Guid? userId, string? userName, AuditActionType actionType,
                      string entityName, Guid? entityId, string? description,
                      string? oldValue = null, string? newValue = null);

        Task<PagedResult<ResultAuditLogDto>> GetPagedAsync(int page, int pageSize);
    }
}
