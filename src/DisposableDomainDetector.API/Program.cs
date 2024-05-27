using DisposableDomainDetector.API.Extensions;
using DisposableDomainDetector.API.Settings;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add webhost config
builder.WebHost.Configure();

// Add host config
builder.Host.Configure();

var configuration = builder.Configuration;
var appSettings = configuration.Get<AppSettings>();

Console.WriteLine(JsonConvert.SerializeObject(appSettings, Formatting.Indented));

// Add services to the contianer.
builder.Services.AddServices(configuration, appSettings);

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
