using CargoTrack.Entity.Entities.Enums;

namespace CargoTrack.DTO.DTOs.CargoDtos
{
    public class CreateCargoDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid OriginBranchId { get; set; }
        public Guid DestinationBranchId { get; set; }

        // Ölçüler
        public double Weight { get; set; }   // kg
        public double Width { get; set; }    // En (cm)
        public double Height { get; set; }   // Yükseklik (cm)
        public double Length { get; set; }   // Boy (cm)

        public CargoType CargoType { get; set; }
        public DeliveryType DeliveryType { get; set; }
    }
}
