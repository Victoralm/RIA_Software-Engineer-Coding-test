using Application.Repositories.Interfaces;
using MediatR;

namespace Application.handlers;

public record CustomerGetMaxIdQuery() : IRequest<int>;

public class CustomerGetMaxIdQueryHandler : IRequestHandler<CustomerGetMaxIdQuery, int>
{
    private readonly ICustomerRepository _repository;

    public CustomerGetMaxIdQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CustomerGetMaxIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.MaxId();

        return result;
    }
}
