using CargoTrack.DTO.DTOs.CargoDtos;
using CargoTrack.DTO.DTOs.CargoMovementDtos;

namespace CargoTrack.DTO.DTOs.DashboardDtos
{
    /// <summary>
    /// Manager dashboard: bugün gelen/çıkan, dağıtımdaki, teslim edilen, geciken ve sorunlu kargolar
    /// ile son hareketler. Yalnızca bağlı olunan şube verisini içerir.
    /// </summary>
    public class ManagerDashboardDto
    {
        public string BranchName { get; set; }

        public int TodayIncomingCount { get; set; }
        public int TodayOutgoingCount { get; set; }
        public int InDistributionCount { get; set; }
        public int DeliveredCount { get; set; }
        public int DelayedCount { get; set; }
        public int ProblematicCount { get; set; }

        public List<ResultCargoMovementDto> RecentMovements { get; set; } = new();
        public List<ResultCargoDto> DistributionList { get; set; } = new();
    }
}
