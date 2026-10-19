namespace CargoTrack.DTO.DTOs.ReportDtos
{
    public class EmployeePerformanceDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string BranchName { get; set; }

        public int DeliveryCount { get; set; }           // Teslimat sayısı
        public int FailedDeliveryCount { get; set; }     // Başarısız teslimat
        public int MovementCount { get; set; }           // Günlük/toplam işlem sayısı
    }
}
