﻿using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}", exception.Message, DateTime.UtcNow);

        int statusCode = exception switch
        {
            InternalServerException => context.Response.StatusCode = StatusCodes.Status500InternalServerError,
            ValidationException => context.Response.StatusCode = StatusCodes.Status400BadRequest,
            BadRequestException => context.Response.StatusCode = StatusCodes.Status400BadRequest,
            NotFoundException => context.Response.StatusCode = StatusCodes.Status404NotFound,
            _ => context.Response.StatusCode = StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Status = statusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
