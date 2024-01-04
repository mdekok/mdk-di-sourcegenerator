using BusinessLogic;
using BusinessBaseLogic;
using Mdk.DISourceGenerator;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

// Registers services of the host and all direct and transitive referenced assemblies.
builder.Services.RegisterServicesMinimalApi();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register IServiceCollection to later iterate over the registered services in /MinimalApiService call.
builder.Services.AddSingleton(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Return source generated registered services.
app.MapGet("/registrations", (IServiceCollection services) =>
    services
        .Where(descriptor =>
            descriptor.ServiceType.Namespace is string ns
                ? ns.StartsWith("MinimalApi") || ns.Contains("Business")
                : false)
        .Select(descriptor => new Registration(
            descriptor.ServiceType.Name,
            descriptor.ImplementationType?.Name,
            descriptor.Lifetime.ToString())));

// Test services in MinimalApi host.
app.MapGet("/MinimalApiService", (MinimalApiService service) => service.GetType().FullName);

// Test services in BusinessLogic assembly.
app.MapGet("/MyService", (MyService service) => service.GetType().FullName);
app.MapGet("/IInterface1", (IInterface1 service) => service.GetType().FullName);
app.MapGet("/MyGenericService", (MyGenericService<int> service) => service.GetType().FullName);
app.MapGet("/IInterface2string", (IInterface2<string> service) => service.GetType().FullName);
app.MapGet("/IInterface2int", (IInterface2<int> service) => service.GetType().FullName); 
app.MapGet("/IInterface3", (IInterface3 service) => service.GetType().FullName);
app.MapGet("/IInterface4", (IInterface4 service) => service.GetType().FullName);

// Test services in BaseBusinessLogic assembly.
app.MapGet("/MyBaseService", (MyBaseService service) => service.GetType().FullName);
app.MapGet("/IBaseInterface", (IBaseInterface service) => service.GetType().FullName);

app.Run();

internal record Registration(string ServiceType, string? ImplementationType, string Lifetime);