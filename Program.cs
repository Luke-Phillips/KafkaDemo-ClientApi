using ClientApi.Data;
using ClientApi.Kafka;
using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/Numbers", async (int number) => {
    using (var db = new ClientApiContext())
    {
        SavedNumber? existingNumber = await db.SavedNumbers.FirstOrDefaultAsync(n => n.Value == number);
        if (existingNumber != null)
        {
            if (existingNumber.IsBanned)
            {
                return Results.Ok("Number is banned");
            }
            return Results.BadRequest("Number already saved");
        }
        await db.AddAsync(new SavedNumber() { Value = number, IsBanned = false });
        await db.SaveChangesAsync();
    }
    KafkaProducer.Produce("numbers", "saveNumber", number.ToString());
    return Results.Created();
})
.WithName("CreateSavedNumber")
.WithOpenApi();

app.MapGet("/Numbers", async () => {
    using (var db = new ClientApiContext())
    {
        return (await db.SavedNumbers.ToListAsync())
            .Where(n => !n.IsBanned)
            .Select(n => n.Value)
            .OrderBy(n => n);
    }
})
.WithName("GetNumbers")
.WithOpenApi();

// app.MapDelete("/Numbers", async () => {
//     using (var db = new ClientApiContext())
//     {
//         await db.SavedNumbers.ForEachAsync(n => db.Remove(n));
//         await db.SaveChangesAsync();
//     }
//     return Results.Ok();
// })
// .WithName("DeleteNumbers")
// .WithOpenApi();

app.Run();