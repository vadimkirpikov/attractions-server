using FluentValidation;
using TouristServer.Presentation.Dto;

namespace TouristServer.Validators;

public class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен быть не менее 6 символов");

        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Обязательно ввести имя")
            .MinimumLength(3)
            .WithMessage("Имя должно состоять из 3 и более символов");
    }
}