using Microsoft.EntityFrameworkCore;
using TodoApp.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

