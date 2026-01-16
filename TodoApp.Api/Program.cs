using Microsoft.EntityFrameworkCore;
using TodoApp.Api;
using TodoApp.Api.Models;

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


app.MapGet("/api/todos", async (
    AppDbContext context,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default) =>
{
    try
    {
        int skip = (pageNumber - 1) * pageSize;
        int totalCount = await context.TodoItems.CountAsync(cancellationToken);
        var todos = await context.TodoItems
            .AsNoTracking()
            .OrderBy(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Results.Ok(new PaginatedResponse<TodoItem>
        {
            Items = todos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
    catch
    {
        return Results.Ok(new PaginatedResponse<TodoItem>
        {
            Items = [],
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 0
        });
    }
});


app.MapGet("/api/todos/completed", async (
    AppDbContext context,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default) =>
{
    try
    {
        int skip = (pageNumber - 1) * pageSize;
        int totalCount = await context.TodoItems.CountAsync(x => x.IsCompleted == true, cancellationToken);
        var todos = await context.TodoItems
            .Where(x => x.IsCompleted)
            .AsNoTracking()
            .OrderBy(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Results.Ok(new PaginatedResponse<TodoItem>
        {
            Items = todos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
    catch
    {
        return Results.Ok(new PaginatedResponse<TodoItem>
        {
            Items = [],
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 0
        });
    }
});


app.MapGet("/api/todos/{id:guid}", async (
    Guid id,
    AppDbContext context,
    CancellationToken cancellationToken) =>
{
    var todo = await context.TodoItems
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    return todo is null ? Results.NotFound() : Results.Ok(todo);
});


app.MapPost("/api/todos", async (
    TodoItem todo,
    AppDbContext context,
    CancellationToken cancellationToken) =>
{
    var newTodo = new TodoItem
    {
        Title = todo.Title,
        Description = todo.Description,
        CreatedAt = DateTime.UtcNow
    };

    context.TodoItems.Add(newTodo);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/todos/{newTodo.Id}", newTodo);
});


app.MapPut("/api/todos/{id:guid}/complete", async (
    Guid id,
    AppDbContext context,
    CancellationToken cancellationToken) =>
{
    var todo = await context.TodoItems.FindAsync([id], cancellationToken);
    if (todo is null) return Results.NotFound();

    todo.IsCompleted = true;
    await context.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
});


app.MapDelete("/api/todos/{id:guid}", async (
    Guid id,
    AppDbContext context,
    CancellationToken cancellationToken) =>
{
    var todo = await context.TodoItems.FindAsync([id], cancellationToken);
    if (todo is null) return Results.NotFound();

    context.TodoItems.Remove(todo);
    await context.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
});

app.Run();

