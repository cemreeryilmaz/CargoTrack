using CargoTrack.DTO.DTOs.CargoDtos;

namespace CargoTrack.DTO.DTOs.DashboardDtos
{
    /// <summary>
    /// Admin dashboard: toplam/aktif/geciken kargo, teslimat oranı, aktif şube ve personel,
    /// son 7 günlük trafik, durum dağılımı, en yoğun 5 şube.
    /// </summary>
    public class AdminDashboardDto
    {
        public int TotalCargoCount { get; set; }
        public int ActiveCargoCount { get; set; }
        public int DelayedCargoCount { get; set; }
        public int DeliveredCargoCount { get; set; }
        public double DeliveryRate { get; set; }       // Teslimat oranı (%)

        public int ActiveBranchCount { get; set; }
        public int ActiveEmployeeCount { get; set; }
        public int UserCount { get; set; }

        public List<ChartPointDto> Last7DaysTraffic { get; set; } = new();
        public List<ChartPointDto> StatusDistribution { get; set; } = new();
        public List<ChartPointDto> Top5BusiestBranches { get; set; } = new();

        public List<ResultCargoDto> RecentCargos { get; set; } = new();
    }
}
