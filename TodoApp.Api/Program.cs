using Microsoft.EntityFrameworkCore;
using TodoApp.Api;
using TodoApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/todos", async (AppDbContext db) => 
    await db.TodoItems.AsNoTracking().ToListAsync());

app.MapGet("/todos/{id:guid}", async (Guid id, AppDbContext db) =>
    await db.TodoItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id));

app.MapPost("/todos", async (AppDbContext db, TodoItem input) =>
{
    var todo = new TodoItem { Title = input.Title };
    db.TodoItems.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

app.MapPut("/todos/{id:guid}/complete", async (Guid id, AppDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.IsCompleted = true;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.TodoItems.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();

