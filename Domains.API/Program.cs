using Domains.API.Data;
using Domains.API.Helpers;
using Domains.API.Managers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<DomainDBContext>(options =>
    options.UseSqlite("Data Source=Data/domains.db"));

var settings = new SettingsHelper(builder.Configuration);
builder.Services.AddSingleton(settings);

builder.Services.AddScoped<DomainManager>();
builder.Services.AddScoped<ValidationHelper>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DomainDBContext>();
    context.Database.EnsureCreated();
}

Console.WriteLine("Listening on http://localhost:5243");
app.MapControllers();
app.Run();

