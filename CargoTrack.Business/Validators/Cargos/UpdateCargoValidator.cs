using CargoTrack.DTO.DTOs.CargoDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Cargos
{
    public class UpdateCargoValidator : AbstractValidator<UpdateCargoDto>
    {
        public UpdateCargoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("Gönderici seçimi zorunludur.");
            RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Alıcı seçimi zorunludur.");
            RuleFor(x => x.OriginBranchId).NotEmpty().WithMessage("Gönderici şube zorunludur.");
            RuleFor(x => x.DestinationBranchId).NotEmpty().WithMessage("Varış şube zorunludur.");

            RuleFor(x => x.DestinationBranchId)
                .NotEqual(x => x.OriginBranchId)
                .WithMessage("Gönderici ve varış şubesi aynı olamaz.");

            RuleFor(x => x.Weight).GreaterThan(0).WithMessage("Ağırlık 0'dan büyük olmalıdır.");
            RuleFor(x => x.Width).GreaterThan(0).WithMessage("En 0'dan büyük olmalıdır.");
            RuleFor(x => x.Height).GreaterThan(0).WithMessage("Yükseklik 0'dan büyük olmalıdır.");
            RuleFor(x => x.Length).GreaterThan(0).WithMessage("Boy 0'dan büyük olmalıdır.");
        }
    }
}
