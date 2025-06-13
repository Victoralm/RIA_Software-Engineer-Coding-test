using Application.Behaviors;
using Application.Repositories.Interfaces;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Repositories.Implementations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(config.GetConnectionString("SQLiteConnection")));

        #region Automatically initializing the tables
        // Keep in mind that it will not change the database after the creation
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        #endregion

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }

}
