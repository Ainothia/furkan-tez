using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // Swagger available at the root
    });
}

app.MapPost("/process-data", async context =>
{
    using var reader = new StreamReader(context.Request.Body);
    var requestBody = await reader.ReadToEndAsync();
    if (string.IsNullOrWhiteSpace(requestBody))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid request: Body cannot be empty.");
        return;
    }

    Console.WriteLine($"Received data: {requestBody}");
    var responseMessage = await ProcessReceivedData(requestBody);
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync($"{{\"response\": \"{responseMessage}\"}}");
});

async Task<string> ProcessReceivedData(string data)
{
    Console.WriteLine($"Processing received data: {data}");
    return $"Processed: {data}";
}

app.Run();