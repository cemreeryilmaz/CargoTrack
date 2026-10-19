using CargoTrack.DTO.DTOs.TransferCenterDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.TransferCenters
{
    public class CreateTransferCenterValidator : AbstractValidator<CreateTransferCenterDto>
    {
        public CreateTransferCenterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Transfer merkezi adı boş bırakılamaz.")
                .MinimumLength(2).WithMessage("Transfer merkezi adı en az 2 karakter olmalıdır.");

            RuleFor(x => x.CityId).NotEmpty().WithMessage("Şehir seçimi zorunludur.");
        }
    }
}
