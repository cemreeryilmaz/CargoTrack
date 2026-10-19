using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.AuditLogDtos
{
    public class ResultAuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public AuditActionType ActionType { get; set; }
        public string ActionTypeName => ActionType.GetDisplayName();

        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public DateTime ActionDate { get; set; }
        public string? Description { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
