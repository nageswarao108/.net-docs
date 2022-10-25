﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MinApiRouteGroupSample;

public static class TodoEndpoints
{
    // <snippet_TodoEndpoints>
    public static RouteGroupBuilder MapTodosApi(this RouteGroupBuilder group, bool isPrivate)
    {
        group.MapGet("/", GetAllTodos);
        group.MapGet("/{id}", GetTodo);
        group.MapPost("/", CreateTodo);
        group.MapPut("/{id}", UpdateTodo);
        group.MapDelete("/{id}", DeleteTodo);

        // create todo
        // <snippet_Create>
        async Task<Created<Todo>> CreateTodo(Todo todo, TodoGroupDbContext database)
        {
            // IsPrivate is always set for /private/todos and never for /public/todos
            todo.IsPrivate = isPrivate;

            await database.Todos.AddAsync(todo);
            await database.SaveChangesAsync();

            return TypedResults.Created($"{todo.Id}", todo);
        }
        // </snippet_Create>

        return group;
    }
    // </snippet_TodoEndpoints>

    // get all todos
    public static async Task<Ok<List<Todo>>> GetAllTodos(TodoGroupDbContext database)
    {
        var todos = await database.Todos.ToListAsync();
        return TypedResults.Ok(todos);
    }

    // get todo by id
    public static async Task<Results<Ok<Todo>, NotFound>> GetTodo(int id, TodoGroupDbContext database)
    {
        var todo = await database.Todos.FindAsync(id);

        if (todo is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(todo);
    }
    // update todo
    public static async Task<Results<NoContent, NotFound>> UpdateTodo(Todo todo, TodoGroupDbContext database)
    {
        var existingTodo = await database.Todos.FindAsync(todo.Id);

        if (existingTodo is null)
            return TypedResults.NotFound();

        existingTodo.Title = todo.Title;
        existingTodo.Description = todo.Description;
        existingTodo.IsDone = todo.IsDone;

        await database.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    // delete todo
    public static async Task<Results<NoContent, NotFound>> DeleteTodo(int id, TodoGroupDbContext database)
    {
        var todo = await database.Todos.FindAsync(id);

        if (todo is null)
            return TypedResults.NotFound();

        database.Todos.Remove(todo);
        await database.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}
