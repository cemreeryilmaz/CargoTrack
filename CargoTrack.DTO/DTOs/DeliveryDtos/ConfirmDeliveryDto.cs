namespace CargoTrack.DTO.DTOs.DeliveryDtos
{
    /// <summary>
    /// Teslimat kodu doğrulama ekranı. Kod doğrulanmadan "Teslim Edildi" durumuna geçilemez.
    /// </summary>
    public class ConfirmDeliveryDto
    {
        public Guid CargoId { get; set; }
        public string DeliveryCode { get; set; }    // 6 haneli kod
        public string ReceivedByName { get; set; }   // Teslim alan kişi
    }
}
