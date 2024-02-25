using ElympicsNet.Api;
using ElympicsNet.Api.Exceptions;
using ElympicsNet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
//For testing sake
app.UseCors(a => a.AllowAnyOrigin());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/podcasts", async (
        [FromServices] IPodcastsService podcastsService,
        [FromServices] Serilog.ILogger log,
        CancellationToken ct
    ) =>
    {
        var response = await podcastsService.ProcessPodcast(ct);
        
        log.Information("Successfully processed data");
        return response.Any() ? Results.Ok(response) : Results.NotFound();
    })
    .WithName("GetPodcasts")
    .Produces<IReadOnlyList<PodcastDto>>()
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces<ExceptionMiddleware.Error>(StatusCodes.Status400BadRequest)
    .WithOpenApi();

app.MapGet("/", (
) => Results.Ok("Health Check"));

app.Run();