using System.ComponentModel.DataAnnotations;

namespace CargoTrack.Entity.Entities.Enums
{
    /// <summary>
    /// Audit log action types.
    /// </summary>
    public enum AuditActionType
    {
        [Display(Name = "Oluşturma")]
        Create = 1,

        [Display(Name = "Güncelleme")]
        Update = 2,

        [Display(Name = "Silme")]
        Delete = 3,

        [Display(Name = "Durum Değişikliği")]
        StatusChange = 4,

        [Display(Name = "Giriş")]
        Login = 5,

        [Display(Name = "Teslimat Onayı")]
        DeliveryConfirmed = 6,

        [Display(Name = "Başarısız Teslimat")]
        DeliveryFailed = 7,

        [Display(Name = "İade")]
        Returned = 8
    }
}
