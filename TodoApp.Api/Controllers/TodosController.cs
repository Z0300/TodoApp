using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Models;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/todos")]
public sealed class TodosController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTodos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
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

            return Ok(new PaginatedResponse<TodoItem>
            {
                Items = todos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        catch (Exception)
        {
            return Ok(new PaginatedResponse<TodoItem>
            {
                Items = [],
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            });
        }
    }

    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedTodos(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            int skip = (pageNumber - 1) * pageSize;

            int totalCount = await context.TodoItems.CountAsync(cancellationToken);
            var todos = await context.TodoItems
                .Where(x => x.IsCompleted == true)
                .AsNoTracking()
                .OrderBy(p => p.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return Ok(new PaginatedResponse<TodoItem>
            {
                Items = todos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        catch (Exception)
        {
            return Ok(new PaginatedResponse<TodoItem>
            {
                Items = [],
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                TotalPages = 0
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoItem>> GetTodo(Guid id, CancellationToken cancellationToken)
    {
        var todo = await context.TodoItems
            .AsNoTracking()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (todo is null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateProduct(
       TodoItem todo,
       CancellationToken cancellationToken)
    {
        var newTodo = new TodoItem { Title = todo.Title, Description = todo.Description };

        context.TodoItems.Add(todo);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(
            nameof(GetTodo),
            new { id = todo.Id },
            todo);
    }


    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> CompleteTodo(
     Guid id,
     CancellationToken cancellationToken)
    {
        var todo = await context.TodoItems.FindAsync([id], cancellationToken);

        if (todo is null) return NotFound();

        todo.IsCompleted = true;
        await context.SaveChangesAsync(cancellationToken);

       return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTodo(
    Guid id,
    CancellationToken cancellationToken)
    {
        var todo = await context.TodoItems.FindAsync([id], cancellationToken);

        if (todo is null) return NotFound();

        context.TodoItems.Remove(todo);
        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
