using CargoTrack.DTO.DTOs.BranchDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Branches
{
    public class CreateBranchValidator : AbstractValidator<CreateBranchDto>
    {
        public CreateBranchValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Şube adı boş bırakılamaz.")
                .MinimumLength(2).WithMessage("Şube adı en az 2 karakter olmalıdır.");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("Şehir seçimi zorunludur.");
        }
    }
}
