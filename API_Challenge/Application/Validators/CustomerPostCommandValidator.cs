using Application.handlers;
using Application.Repositories.Interfaces;
using FluentValidation;

namespace Application.Validators;

public class CustomerPostCommandValidator : AbstractValidator<CustomerPostCommand>
{
    private readonly ICustomerRepository _repository;

    public CustomerPostCommandValidator(ICustomerRepository repository)
    {
        _repository = repository;

        // Preventing Race Condition - Validation
        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellation) => await _repository.ExistsAsync(id))
            .WithMessage("The given Id already exist.");

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(x => x.Age).NotEmpty().WithMessage("Age is required")
            .GreaterThan(18).WithMessage("Age must be greater than 18");
    }
}