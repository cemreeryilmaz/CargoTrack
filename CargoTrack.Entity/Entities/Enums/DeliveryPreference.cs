using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    /// <summary>
    /// Customer delivery preference chosen while the cargo is out for delivery.
    /// </summary>
    public enum DeliveryPreference
    {
        [Display(Name = "Seçilmedi")]
        None = 0,

        [Display(Name = "Adresteyim")]
        AtAddress = 1,

        [Display(Name = "Komşuya Teslim")]
        DeliverToNeighbor = 2,

        [Display(Name = "Randevu Oluşturuldu")]
        Appointment = 3
    }
}
