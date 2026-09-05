namespace CargoTrack.Entity.Entities.Enums
{
    public enum CargoStatus
    {
        Received = 1, // Kargo Teslim alındı.
        InTransferCenter = 2, // Kargo transfer merkezinde.
        DispatchedFromTransferCenter = 3, // Kargo transfer merkezinden ayrıldı.
        ArrivedAtDeliveryBranch = 4, // Kargo dağıtım şubesine ulaştı.
        OutForDelivery = 5, // Kargo teslimat için yola çıktı.
        Delivered = 6, // Kargo teslim edildi.
    }
}
