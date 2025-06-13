using Application.Repositories.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.handlers;

public record CustomerPostCommand(int Id, string? FirstName, string? LastName, int Age);
public record CustomerBulkPostCommand(IEnumerable<CustomerPostCommand> Customers) : IRequest<bool>;

public class CustomerPostCommandHandler : IRequestHandler<CustomerBulkPostCommand, bool>
{
    private readonly ICustomerRepository _repository;

    public CustomerPostCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CustomerBulkPostCommand request, CancellationToken cancellationToken)
    {
        // Mapping CustomerPostCommand to Customer
        var customers = request.Customers.Select(c => new Customer
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Age = c.Age
        });

        var result = await _repository.BulkAddAsync(customers);

        return result;
    }
}
