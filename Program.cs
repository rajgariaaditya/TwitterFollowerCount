// https://cdn.syndication.twimg.com/widgets/followbutton/info.json?screen_names=supermadgg
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup => setup.SwaggerDoc("v1", new OpenApiInfo()
{
    Description = "Twitter follower count web api implementation using Minimal Api in Asp.Net Core",
    Title = "Twitter Follower Count API",
    Version = "v1",
    Contact = new OpenApiContact()
    {
        Name = "Aditya Rajgaria",
        Url = new Uri("https://github.com/rajgariaaditya")
    }
}));
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet("/followerCount/{twitterHandle}", async (string twitterHandle, IHttpClientFactory clientFactory) =>
{
    if(twitterHandle?.Split(',').Length > 1) {
        return Results.BadRequest("Only 1 twitter handle is allowed at a time");
    }

    var client = clientFactory.CreateClient();    
    var request = new HttpRequestMessage(HttpMethod.Get, $"https://cdn.syndication.twimg.com/widgets/followbutton/info.json?screen_names={twitterHandle}");
    
    var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<object>(responseStream);
        }

    return Results.StatusCode(500);
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Twitter Follower Count API v1");
    c.RoutePrefix = string.Empty;
});

app.Run();
