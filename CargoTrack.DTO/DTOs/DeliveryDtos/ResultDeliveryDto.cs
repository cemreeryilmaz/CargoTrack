namespace CargoTrack.DTO.DTOs.DeliveryDtos
{
    public class ResultDeliveryDto
    {
        public Guid Id { get; set; }
        public Guid CargoId { get; set; }
        public string DeliveryCode { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? ReceivedByName { get; set; }
        public string? EmployeeName { get; set; }
    }
}
