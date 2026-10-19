using CargoTrack.DataAccess.Repositories.AuditLogs;
using CargoTrack.DTO.DTOs.AuditLogDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;
using Mapster;

namespace CargoTrack.Business.Services.AuditLogs
{
    public class AuditLogService(IAuditLogRepository _auditLogRepository) : IAuditLogService
    {
        public async Task LogAsync(Guid? userId, string? userName, AuditActionType actionType,
                                   string entityName, Guid? entityId, string? description,
                                   string? oldValue = null, string? newValue = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                UserName = userName ?? "System",
                ActionType = actionType,
                EntityName = entityName,
                EntityId = entityId,
                ActionDate = DateTime.Now,
                Description = description ?? string.Empty,
                OldValue = oldValue ?? string.Empty,
                NewValue = newValue ?? string.Empty
            };

            await _auditLogRepository.CreateAsync(log);
        }

        public async Task<PagedResult<ResultAuditLogDto>> GetPagedAsync(int page, int pageSize)
        {
            var paged = await _auditLogRepository.GetPagedAsync(page, pageSize);

            return new PagedResult<ResultAuditLogDto>
            {
                Items = paged.Items.Adapt<List<ResultAuditLogDto>>(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }
    }
}
