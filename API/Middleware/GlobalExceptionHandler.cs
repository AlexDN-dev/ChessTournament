using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            NotFoundException   => (StatusCodes.Status404NotFound,         "Ressource introuvable"),
            ConflictException   => (StatusCodes.Status409Conflict,         "Conflit"),
            ValidationException => (StatusCodes.Status400BadRequest,       "Erreur de validation"),
            _                   => (StatusCodes.Status500InternalServerError, "Erreur interne"),
        };

        if (status >= 500)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning(exception, "{Title}: {Message}", title, exception.Message);

        var problem = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = exception is ChessTournamentException ? exception.Message : "Une erreur inattendue est survenue.",
            Type   = $"https://httpstatuses.io/{status}"
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
