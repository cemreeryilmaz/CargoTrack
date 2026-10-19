using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.Entity.Entities
{
    /// <summary>
    /// Transfer merkezi. Kargo iki şube arasında ilerlerken uğradığı aktarma noktası.
    /// </summary>
    public class TransferCenter : BaseEntity
    {
        public string Name { get; set; }
        public Guid CityId { get; set; }

        // Navigation properties
        public virtual City City { get; set; }
        public virtual IList<CargoMovement> CargoMovements { get; set; }
    }
}
