using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add dependency injection services - AddServices
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure pipeline - UseMethod

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.TodoItems.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.TodoItems.FindAsync(id));

app.MapPost("/todoitems", async (TodoItem todo, TodoDb db) =>
{
    await db.TodoItems.AddAsync(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem inputTodo, TodoDb db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }
    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.TodoItems.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }
    db.TodoItems.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();