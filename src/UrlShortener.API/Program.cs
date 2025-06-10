using Cassandra;
using UrlShortener.API.Data.Repositories;
using UrlShortener.API.Middlewares;
using UrlShortener.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var cluster = Cluster.Builder()
         .AddContactPoint("127.0.0.1")//"urlshortener.database")
         .WithDefaultKeyspace("UrlShortenerKeyspace")
         .Build();

builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

builder.Services.AddScoped(_ => cluster.ConnectAndCreateDefaultKeyspaceIfNotExists());

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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
