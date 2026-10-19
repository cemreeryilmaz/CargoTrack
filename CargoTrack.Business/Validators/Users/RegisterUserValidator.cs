using CargoTrack.DTO.DTOs.UserDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrack.Business.Validators.Users
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad alanı boş bırakılamaz.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Kullanıcı adı alanı boş bırakılamaz.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta alanı boş bırakılamaz.")
                                 .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre alanı boş bırakılamaz.");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Şifre tekrarı alanı boş bırakılamaz.")
                                           .Matches(x => x.Password).WithMessage("Şifreler birbirleriyle uyumlu değil.");
        }
    }
}
