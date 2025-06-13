using Domain.Entities;

namespace Application.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<bool> BulkAddAsync(IEnumerable<Customer> customers);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<int> MaxId();
    Task<bool> ExistsAsync(int id);
}
