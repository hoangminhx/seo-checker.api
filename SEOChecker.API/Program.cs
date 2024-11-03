using SEOChecker.API.Configs;
using SEOChecker.Application.Configs;
using SEOChecker.Domain.AppSettingsModels;
using SEOChecker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.Configure<SearchEnginesSettingModel>(configuration.GetSection("App:SearchEngines"));
builder.Services.Configure<AppSettingsModel>(configuration.GetSection("App:Settings"));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCorsWithConfig(configuration);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyForMediatR).Assembly);
}
);
builder.Services.AddMemoryCache();
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
