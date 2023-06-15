using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Domain.Core.Events;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Infra.IoC;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BankingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankingDbConnection"));
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RabbitMQBus).Assembly));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

RegisterServices(builder.Services);

void RegisterServices(IServiceCollection services)
{
    DependencyContainer.RegisterServices(services);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();