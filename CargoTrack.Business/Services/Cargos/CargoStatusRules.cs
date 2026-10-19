using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.Business.Services.Cargos
{
    /// <summary>
    /// Kargo durum geçiş kuralları (state machine). İzin verilen geçişler burada tanımlanır.
    /// </summary>
    public static class CargoStatusRules
    {
        private static readonly Dictionary<CargoStatus, List<CargoStatus>> AllowedTransitions = new()
        {
            [CargoStatus.Created] = new() { CargoStatus.AtOriginBranch },
            [CargoStatus.AtOriginBranch] = new() { CargoStatus.AtTransferCenter, CargoStatus.AtDestinationBranch },
            [CargoStatus.AtTransferCenter] = new() { CargoStatus.AtTransferCenter, CargoStatus.AtDestinationBranch },
            [CargoStatus.AtDestinationBranch] = new() { CargoStatus.OutForDelivery },
            [CargoStatus.OutForDelivery] = new() { CargoStatus.Delivered, CargoStatus.DeliveryFailed },
            [CargoStatus.DeliveryFailed] = new() { CargoStatus.OutForDelivery, CargoStatus.InReturnProcess },
            [CargoStatus.InReturnProcess] = new() { CargoStatus.ReturnedToSender },
            [CargoStatus.Delivered] = new(),
            [CargoStatus.ReturnedToSender] = new()
        };

        public static bool CanTransition(CargoStatus from, CargoStatus to)
        {
            return AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
        }

        public static List<CargoStatus> GetNextStatuses(CargoStatus from)
        {
            return AllowedTransitions.TryGetValue(from, out var targets) ? targets : new List<CargoStatus>();
        }
    }
}
