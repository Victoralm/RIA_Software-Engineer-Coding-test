using API.DI;
using Application.DI;
using Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services
   .AddHelperServices()
   .AddApplication()
   .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

app.Run();

public partial class Program { }
