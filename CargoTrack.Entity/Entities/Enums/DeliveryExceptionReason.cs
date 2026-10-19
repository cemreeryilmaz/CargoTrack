using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    /// <summary>
    /// Delivery failure / problem cargo reasons.
    /// </summary>
    public enum DeliveryExceptionReason
    {
        [Display(Name = "Adres Hatalı")]
        WrongAddress = 1,

        [Display(Name = "Alıcıya Ulaşılamadı")]
        RecipientUnavailable = 2,

        [Display(Name = "Kargo Hasarlı")]
        Damaged = 3,

        [Display(Name = "Alıcı Reddetti")]
        Rejected = 4,

        [Display(Name = "Diğer")]
        Other = 5
    }
}
