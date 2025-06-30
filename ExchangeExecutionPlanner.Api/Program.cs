using System.Reflection;
using ExchangeExecutionPlanner.Repositories;
using ExchangeExecutionPlanner.Services;

var builder = WebApplication.CreateBuilder(args);

// Register repo and service with folder path from config
builder.Services.AddScoped<IExchangeRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var folder = config["ExchangeDataFolder"] ?? "Data/Exchanges";
    return new JsonExchangeRepository(folder);
});
builder.Services.AddScoped<IExchangeExecutionService, ExchangeExecutionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Include XML comments from documentation file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();