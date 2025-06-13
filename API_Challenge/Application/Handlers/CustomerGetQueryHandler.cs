using Application.Repositories.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.handlers;

public record CustomerGetQuery() : IRequest<IEnumerable<Customer>>;

public class CustomerGetQueryHandler : IRequestHandler<CustomerGetQuery, IEnumerable<Customer>>
{
    private readonly ICustomerRepository _repository;

    public CustomerGetQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Customer>> Handle(CustomerGetQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync();

        return result;
    }
}
