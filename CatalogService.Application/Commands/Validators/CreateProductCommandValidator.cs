using FluentValidation;

namespace CatalogService.Application.Commands.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom du produit est obligatoire")
            .MaximumLength(200).WithMessage("Le nom ne peut pas dépasser 200 caractères");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La description est obligatoire")
            .MaximumLength(1000).WithMessage("La description ne peut pas dépasser 1000 caractères");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Le prix doit être supérieur à 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Le stock ne peut pas être négatif");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La catégorie est obligatoire")
            .MaximumLength(100).WithMessage("La catégorie ne peut pas dépasser 100 caractères");
    }
}
