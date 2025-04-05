using TouristServer.Presentation.Dto;

namespace TouristServer.Validators;

using FluentValidation;

public class FilterDtoValidator : AbstractValidator<FilterDto>
{
    public FilterDtoValidator()
    {
        RuleFor(x => x.ConstMin)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ConstMin.HasValue)
            .WithMessage("Минимальное значение должно быть неотрицательным");

        RuleFor(x => x.ConstMax)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ConstMax.HasValue)
            .WithMessage("Максимальное значение должно быть неотрицательным");

        RuleFor(x => x)
            .Must(x =>
                (!x.ConstMin.HasValue || !x.ConstMax.HasValue) ||
                x.ConstMin <= x.ConstMax)
            .WithMessage("ConstMin не может быть больше ConstMax");
    }
}
