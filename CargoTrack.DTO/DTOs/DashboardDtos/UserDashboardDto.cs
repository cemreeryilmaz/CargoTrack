using CargoTrack.DTO.DTOs.CargoDtos;

namespace CargoTrack.DTO.DTOs.DashboardDtos
{
    /// <summary>
    /// User dashboard: aktif kargolar, geçmiş teslimatlar, son kargo durumu,
    /// hareket zaman çizelgesi ve tahmini teslim tarihi.
    /// </summary>
    public class UserDashboardDto
    {
        public int ActiveCargoCount { get; set; }
        public int PastDeliveryCount { get; set; }
        public int SentCargoCount { get; set; }
        public int ReceivedCargoCount { get; set; }

        public List<ResultCargoDto> ActiveCargos { get; set; } = new();
        public CargoDetailDto? LastCargo { get; set; }
    }
}
