using CargoTrack.DTO.DTOs.EmployeeDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators.Employees
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş bırakılamaz.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş bırakılamaz.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Unvan boş bırakılamaz.");
            RuleFor(x => x.BranchId).NotEmpty().WithMessage("Şube seçimi zorunludur.");
        }
    }
}
