namespace CargoTrack.DTO.DTOs.ReportDtos
{
    /// <summary>
    /// Admin performans raporu özeti.
    /// </summary>
    public class PerformanceReportDto
    {
        public double AverageDeliveryDurationDays { get; set; }  // Ortalama teslimat süresi (gün)
        public double OnTimeDeliveryRate { get; set; }           // Zamanında teslimat oranı (%)
        public double ReturnRate { get; set; }                   // İade oranı (%)

        public List<BranchPerformanceDto> BranchPerformances { get; set; } = new();
        public List<EmployeePerformanceDto> EmployeePerformances { get; set; } = new();

        public BranchPerformanceDto? TopSenderBranch { get; set; }    // En fazla kargo gönderen şube
        public BranchPerformanceDto? TopDelivererBranch { get; set; } // En fazla teslimat yapan şube
        public BranchPerformanceDto? MostDelayedBranch { get; set; }  // En fazla geciken kargolu şube
    }
}
