using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql")));

var app = builder.Build();

app.UseCors("AllowMyApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/items", async (ToDoDbContext db) =>
{
    return Results.Ok(await db.Items.ToListAsync());
});

app.MapPost("/items", async (Item newItem, ToDoDbContext db) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

app.MapPut("/items/{id}", async (int id, Item updatedItem, ToDoDbContext db) =>
{
    var existingItem = await db.Items.FindAsync(id);
    if (existingItem is null)
    {
        return Results.NotFound();
    }

    existingItem.Name = updatedItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null)
    {
        return Results.NotFound();
    }

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/", ()=>"AuthServer API is running");

app.Run();
