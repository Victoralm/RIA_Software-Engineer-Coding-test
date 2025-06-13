using Application.handlers;
using Application.Repositories.Interfaces;
using FluentValidation;

namespace Application.Validators;

public class CustomerBulkPostCommandValidator : AbstractValidator<CustomerBulkPostCommand>
{
    public CustomerBulkPostCommandValidator(ICustomerRepository repository)
    {
        RuleForEach(x => x.Customers).SetValidator(new CustomerPostCommandValidator(repository));
    }
}
