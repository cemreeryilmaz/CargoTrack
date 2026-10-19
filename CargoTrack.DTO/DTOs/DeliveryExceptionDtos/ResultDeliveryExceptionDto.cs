using CargoTrack.Entity.Entities.Enums;
using CargoTrack.Entity.Extensions;

namespace CargoTrack.DTO.DTOs.DeliveryExceptionDtos
{
    public class ResultDeliveryExceptionDto
    {
        public Guid Id { get; set; }
        public Guid CargoId { get; set; }
        public string? CargoTrackCode { get; set; }

        public DeliveryExceptionReason Reason { get; set; }
        public string ReasonName => Reason.GetDisplayName();

        public string? Description { get; set; }
        public DateTime ExceptionDate { get; set; }
        public int AttemptNumber { get; set; }
        public string? EmployeeName { get; set; }
    }
}
