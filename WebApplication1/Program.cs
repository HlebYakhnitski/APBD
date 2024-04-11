using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Add endpoints here

// Endpoint to get all animals
app.MapGet("/animals", () => DataStore.Animals);

// Endpoint to get a specific animal by id
app.MapGet("/animals/{id}", (int id) => DataStore.Animals.FirstOrDefault(a => a.Id == id));

// Endpoint to add a new animal
app.MapPost("/animals", (Animal animal) => {
    DataStore.Animals.Add(animal);
    return Results.Created($"/animals/{animal.Id}", animal);
});

// Endpoint to edit an animal
app.MapPut("/animals/{id}", (int id, Animal update) => {
    var animal = DataStore.Animals.FirstOrDefault(a => a.Id == id);
    if (animal is null) return Results.NotFound();
    animal.Name = update.Name;
    animal.Category = update.Category;
    animal.Weight = update.Weight;
    animal.FurColor = update.FurColor;
    return Results.NoContent();
});

// Endpoint to delete an animal
app.MapDelete("/animals/{id}", (int id) => {
    var animal = DataStore.Animals.FirstOrDefault(a => a.Id == id);
    if (animal is null) return Results.NotFound();
    DataStore.Animals.Remove(animal);
    return Results.Ok(animal);
});

// Endpoint to get all visits for an animal
app.MapGet("/animals/{animalId}/visits", (int animalId) => 
    DataStore.Visits.Where(v => v.AnimalId == animalId));

// Endpoint to add a visit for an animal
app.MapPost("/visits", (Visit visit) => {
    DataStore.Visits.Add(visit);
    return Results.Created($"/visits/{visit.Id}", visit);
});

app.Run();