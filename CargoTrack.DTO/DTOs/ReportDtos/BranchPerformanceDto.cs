namespace CargoTrack.DTO.DTOs.ReportDtos
{
    public class BranchPerformanceDto
    {
        public Guid BranchId { get; set; }
        public string BranchName { get; set; }
        public string CityName { get; set; }

        public int SentCargoCount { get; set; }          // Gönderilen kargo
        public int DeliveredCargoCount { get; set; }     // Teslim edilen kargo
        public int DelayedCargoCount { get; set; }       // Geciken kargo
        public double DeliverySuccessRate { get; set; }  // Teslimat başarı oranı (%)
    }
}
