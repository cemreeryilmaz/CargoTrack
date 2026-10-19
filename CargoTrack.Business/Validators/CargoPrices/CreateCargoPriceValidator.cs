using CargoTrack.DTO.DTOs.CargoPriceDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.CargoPrices
{
    public class CreateCargoPriceValidator : AbstractValidator<CreateCargoPriceDto>
    {
        public CreateCargoPriceValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Kural adı boş bırakılamaz.");
            RuleFor(x => x.DeliveryType).IsInEnum().WithMessage("Geçerli bir teslimat tipi seçiniz.");
            RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0).WithMessage("Taban ücret negatif olamaz.");
            RuleFor(x => x.PricePerKg).GreaterThanOrEqualTo(0).WithMessage("Kg ücreti negatif olamaz.");
            RuleFor(x => x.PricePerDesi).GreaterThanOrEqualTo(0).WithMessage("Desi ücreti negatif olamaz.");
            RuleFor(x => x.DifferentCityFee).GreaterThanOrEqualTo(0).WithMessage("Şehir farkı ücreti negatif olamaz.");
        }
    }
}
