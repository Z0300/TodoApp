namespace TodoApp.Api.Models;

public class TotalCountResponse
{
    public int AllTodosCount { get; init; } = 0;
    public int CompletedTodosCount { get; init; } = 0;
}
