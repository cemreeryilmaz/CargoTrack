using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    /// <summary>
    /// Cargo lifecycle statuses. Transitions are validated by business rules in CargoService.
    /// </summary>
    public enum CargoStatus
    {
        [Display(Name = "Oluşturuldu")]
        Created = 1,

        [Display(Name = "Gönderici Şubesinde")]
        AtOriginBranch = 2,

        [Display(Name = "Transfer Merkezinde")]
        AtTransferCenter = 3,

        [Display(Name = "Varış Şubesinde")]
        AtDestinationBranch = 4,

        [Display(Name = "Dağıtıma Çıktı")]
        OutForDelivery = 5,

        [Display(Name = "Teslim Edildi")]
        Delivered = 6,

        // Alternative flows
        [Display(Name = "Teslim Edilemedi")]
        DeliveryFailed = 7,

        [Display(Name = "İade Sürecinde")]
        InReturnProcess = 8,

        [Display(Name = "Göndericiye İade Edildi")]
        ReturnedToSender = 9
    }
}
