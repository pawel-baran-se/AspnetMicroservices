using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddScoped<BasketCheckoutConsumer>();
//MassTransit-RabitMQ Configuration
builder.Services.AddMassTransit(config =>
{
	config.AddConsumer<BasketCheckoutConsumer>();
	config.UsingRabbitMq((ctx, cfg) =>
	{
		cfg.Host(builder.Configuration.GetValue<string>("EventBusSettings:HostAddress"));

		cfg.ReceiveEndpoint(EventBusConstants.BasketChekoutQueue, conf =>
		{
			conf.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
		});
	});
});

//Automapper
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(Program)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MigrateDatabase<OrderContext>((context, services) =>
{
	var logger = services.GetService<ILogger<OrderContextSeed>>();
	OrderContextSeed
		.SeedAsync(context, logger)
		.Wait();
});

app.UseAuthorization();

app.MapControllers();

app.Run();