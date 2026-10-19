namespace CargoTrack.Entity.Entities.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }

        // Audit Fields
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            IsDeleted = false;
        }
    }
}
