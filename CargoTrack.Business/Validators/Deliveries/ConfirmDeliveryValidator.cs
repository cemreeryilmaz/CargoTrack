using CargoTrack.DTO.DTOs.DeliveryDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Deliveries
{
    public class ConfirmDeliveryValidator : AbstractValidator<ConfirmDeliveryDto>
    {
        public ConfirmDeliveryValidator()
        {
            RuleFor(x => x.CargoId).NotEmpty();
            RuleFor(x => x.DeliveryCode)
                .NotEmpty().WithMessage("Teslimat kodu zorunludur.")
                .Length(6).WithMessage("Teslimat kodu 6 haneli olmalıdır.");

            RuleFor(x => x.ReceivedByName)
                .NotEmpty().WithMessage("Teslim alan kişi bilgisi zorunludur.");
        }
    }
}
