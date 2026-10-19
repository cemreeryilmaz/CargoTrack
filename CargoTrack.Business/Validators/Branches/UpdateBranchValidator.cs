using CargoTrack.DTO.DTOs.BranchDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Business.Validators.Branches
{
    public class UpdateBranchValidator : AbstractValidator<UpdateBranchDto>
    {
        public UpdateBranchValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Şube adı boş bırakılamaz.")
                .MinimumLength(2).WithMessage("Şube adı en az 2 karakter olmalıdır.");

            RuleFor(x => x.CityId)
                .NotEmpty().WithMessage("Şehir seçimi zorunludur.");
        }
    }
}
