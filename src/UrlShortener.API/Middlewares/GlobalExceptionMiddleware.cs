﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using UrlShortener.API.Services.Exceptions;

namespace UrlShortener.API.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public GlobalExceptionHandlingMiddleware(
        ILogger<GlobalExceptionHandlingMiddleware> logger) =>
        _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);


            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ProblemDetails problem = new()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server error",
                Title = "Server error",
                Detail = "An internal server has occurred"
            };

            if (e is BaseException baseException)
            {
                problem.Status = (int)baseException.StatusCode;
                problem.Title = baseException.Title;
                problem.Detail = baseException.Message;
                context.Response.StatusCode = (int)baseException.StatusCode;
            }

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);

        }
    }
}
