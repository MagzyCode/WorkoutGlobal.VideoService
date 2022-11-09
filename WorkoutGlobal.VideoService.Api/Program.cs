<<<<<<< Updated upstream
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
=======
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using WorkoutGlobal.VideoService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
// using deprecated AddFluentValidation method for auto validation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddFluentValidation(configuration =>
//    {
//        configuration.RegisterValidatorsFromAssemblyContaining<Program>();
//        configuration.DisableDataAnnotationsValidation = true;
//    });
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureAttributes();
>>>>>>> Stashed changes
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
