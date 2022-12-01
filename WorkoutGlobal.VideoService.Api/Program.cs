using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WorkoutGlobal.VideoService.Api.Extensions;
using MassTransit;
using WorkoutGlobal.Shared.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(setupAction =>
{
    setupAction
        .AddPolicy( "AllowOrigin", options => options
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.ConfigureRepositories();
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureAttributes();
builder.Services.ConfigureMassTransit(builder.Configuration);

var app = builder.Build();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var bus = Bus.Factory.CreateUsingRabbitMq(config =>
{
    config.Host(builder.Configuration["MassTransitSettings:Host"]);

    config.ReceiveEndpoint(builder.Configuration["MassTransitSettings:UpdateQueue"], c =>
    {
        c.Handler<UpdateUserMessage>(async ctx => await Console.Out.WriteLineAsync(ctx.Message.UpdationId.ToString()));
    });
    config.ReceiveEndpoint(builder.Configuration["MassTransitSettings:DeleteQueue"], c =>
    {
        c.Handler<DeleteUserMessage>(async ctx => await Console.Out.WriteLineAsync(ctx.Message.DeletionId.ToString()));
    });
    config.ReceiveEndpoint(builder.Configuration["MassTransitSettings:UpdateVideo"], c =>
    {
        c.Handler<UpdateVideoMessage>(async ctx => await Console.Out.WriteLineAsync(ctx.Message.UpdationId));
    });
    config.ReceiveEndpoint(builder.Configuration["MassTransitSettings:DeleteVideo"], c =>
    {
        c.Handler<DeleteVideoMessage>(async ctx => await Console.Out.WriteLineAsync(ctx.Message.DeletedId));
    });
});

bus.Start();

app.Run();
