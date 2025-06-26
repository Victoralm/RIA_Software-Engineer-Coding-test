using Application.Repositories.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(AppDbContext context, ILogger<CustomerRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> BulkAddAsync(IEnumerable<Customer> customers)
    {
        if (_context.Customers == null)
            throw new InvalidOperationException("Customer DbSet is not initialized.");

        // Declaring the transaction variable outside the try block to ensure it is in scope.
        using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

        try
        {
            await _context.Customers.AddRangeAsync(customers);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        // Preventing Race Condition - Optimistic Concurrency (Best Practice)
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            await transaction.RollbackAsync(); // Explicit rollback - Not mandatory, but it can save a few milliseconds by not waiting for Dispose()
            _logger.LogWarning(ex, "Attempt to insert duplicate ID.");
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(); // Explicit rollback - Not mandatory, but it can save a few milliseconds by not waiting for Dispose()
            _logger.LogError(ex, "Error while adding Customer: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        if (_context.Customers == null)
            throw new InvalidOperationException("Customer DbSet is not initialized.");

        try
        {
            var dbCustomers = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            var ordered = Customer.OrderByName(dbCustomers);

            return ordered;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting Customers: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<int> MaxId()
    {
        var idMax = await _context.Customers.MaxAsync(e => (int?)e.Id);

        return idMax ?? -1;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        if (_context.Customers == null)
            throw new InvalidOperationException("Customer DbSet is not initialized.");

        try
        {
            var result = await _context.Customers.AnyAsync(c => c.Id == id);

            return !result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking Customer Id: {Message}", ex.Message);
            return false;
        }
    }
}
