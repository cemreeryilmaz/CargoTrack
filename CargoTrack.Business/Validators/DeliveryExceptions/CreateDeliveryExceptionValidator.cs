using CargoTrack.DTO.DTOs.DeliveryExceptionDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.DeliveryExceptions
{
    public class CreateDeliveryExceptionValidator : AbstractValidator<CreateDeliveryExceptionDto>
    {
        public CreateDeliveryExceptionValidator()
        {
            RuleFor(x => x.CargoId).NotEmpty();
            RuleFor(x => x.Reason).IsInEnum().WithMessage("Geçerli bir neden seçiniz.");
        }
    }
}
