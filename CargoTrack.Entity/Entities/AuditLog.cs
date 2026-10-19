using CargoTrack.Entity.Entities.Common;
using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Denetim kaydı. Kullanıcı, işlem türü, Entity, Entity ID, işlem tarihi ve açıklama tutulur.
    /// Durum değişiklikleri eski ve yeni değerleriyle izlenebilir.
    /// </summary>
    public class AuditLog : BaseEntity
    {
        public Guid? UserId { get; set; }
        public string UserName { get; set; }

        public AuditActionType ActionType { get; set; }
        public string EntityName { get; set; }
        public Guid? EntityId { get; set; }

        public DateTime ActionDate { get; set; }
        public string Description { get; set; }

        // Durum değişikliklerinde eski/yeni değer
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
