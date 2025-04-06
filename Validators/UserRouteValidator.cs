using FluentValidation;
using TouristServer.Presentation.Dto;

namespace TouristServer.Validators;

public class UserRouteValidator: AbstractValidator<UserRouteDto>
{
    public UserRouteValidator()
    {
        RuleFor(x => x.RoutePlaces)
            .NotEmpty()
            .WithMessage("В маршруте должно быть минимум 1 достопримечательность");
    }
}