using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    /// <summary>
    /// Delivery type. Affects pricing and SLA calculation.
    /// </summary>
    public enum DeliveryType
    {
        [Display(Name = "Standart")]
        Standard = 1,

        [Display(Name = "Ekspres")]
        Express = 2,

        [Display(Name = "Aynı Gün")]
        SameDay = 3
    }
}
