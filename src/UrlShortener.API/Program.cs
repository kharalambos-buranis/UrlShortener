using Cassandra;
using FluentValidation;
using FluentValidation.AspNetCore;
using UrlShortener.API.BackgroundServices;
using UrlShortener.API.Data;
using UrlShortener.API.Data.Repositories;
using UrlShortener.API.Data.Repositories.UrlShortener.API.Data.Repositories;
using UrlShortener.API.Middlewares;
using UrlShortener.API.Services;
using UrlShortener.API.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var cassandraOptions = builder.Configuration.GetSection("Cassandra").Get<CassandraOptions>();

var cluster = Cluster.Builder()
        .AddContactPoint(cassandraOptions.ContactPoint)
        .WithPort(cassandraOptions.Port)
        .WithDefaultKeyspace(cassandraOptions.Keyspace) 
        .Build();

builder.Services.AddScoped(_ =>
{
    var session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists();

    var migrationService = new CassandraMigrationService(session);
    migrationService.Migrate();

    return session;
});

builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
builder.Services.AddScoped<IUrlClickAnalyticsRepository, UrlClickAnalyticsReposiotry>();

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<ShortenUrlRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<ShortenUrlRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHostedService<InactivateExpiredUrlService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
