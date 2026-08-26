using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Api.Responses;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;

namespace ValenceHub.Api.Extensions;

public static class ResultHttpExtensions
{
    public static IActionResult ToApiResponse<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        var now = controller.GetCurrentUtcNow();

        if (result.IsSuccess)
        {
            return controller.Ok(
                ApiResponse<T>.Ok(
                    result.Value!,
                    controller.HttpContext.TraceIdentifier,
                    now));
        }

        return controller.ToProblem(result.Error);
    }

    public static IActionResult ToApiResponse(
        this Result result,
        ControllerBase controller)
    {
        var now = controller.GetCurrentUtcNow();

        if (result.IsSuccess)
        {
            return controller.Ok(
                ApiResponse<object?>.Ok(
                    null,
                    controller.HttpContext.TraceIdentifier,
                    now));
        }

        return controller.ToProblem(result.Error);
    }

    public static IActionResult ToApiResponse(
        this Result<Unit> result,
        ControllerBase controller)
    {
        var now = controller.GetCurrentUtcNow();

        if (result.IsSuccess)
        {
            return controller.Ok(
                ApiResponse<object?>.Ok(
                    null,
                    controller.HttpContext.TraceIdentifier,
                    now));
        }

        return controller.ToProblem(result.Error);
    }

    public static IActionResult ToCreatedResponse<T>(
        this Result<T> result,
        ControllerBase controller,
        string actionName,
        object routeValues)
    {
        var now = controller.GetCurrentUtcNow();

        if (result.IsFailure)
            return controller.ToProblem(result.Error);

        return controller.CreatedAtAction(
            actionName,
            routeValues,
            ApiResponse<T>.Ok(
                result.Value!,
                controller.HttpContext.TraceIdentifier,
                now));
    }

    public static IActionResult ToCreatedResponse<T>(
        this Result<T> result,
        ControllerBase controller,
        string locationUri)
    {
        var now = controller.GetCurrentUtcNow();

        if (result.IsFailure)
            return controller.ToProblem(result.Error);

        return controller.Created(
            locationUri,
            ApiResponse<object?>.Ok(
                null,
                controller.HttpContext.TraceIdentifier,
                now));
    }

    public static IActionResult ToResponse<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        var now = controller.GetCurrentUtcNow();

        if (result.IsFailure)
            return controller.ToProblem(result.Error);

        return controller.Ok(
            ApiResponse<object?>.Ok(
                null,
                controller.HttpContext.TraceIdentifier,
                now));
    }

    private static IActionResult ToProblem(
        this ControllerBase controller,
        Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotSupported => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        ProblemDetails details = error.Type == ErrorType.Validation
            ? CreateValidationProblemDetails(controller, error, statusCode)
            : new ProblemDetails
            {
                Title = error.Type.ToString(),
                Detail = error.Message,
                Status = statusCode,
                Instance = controller.HttpContext.Request.Path
            };

        details.Extensions["code"] = error.Code;
        details.Extensions["metadata"] = error.Metadata;
        details.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;
        details.Extensions["timestampUtc"] = controller.GetCurrentUtcNow();

        return controller.StatusCode(statusCode, details);
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        ControllerBase controller,
        Error error,
        int statusCode)
    {
        var errors = ExtractValidationErrors(error);

        return new ValidationProblemDetails(errors)
        {
            Title = error.Type.ToString(),
            Detail = error.Message,
            Status = statusCode,
            Instance = controller.HttpContext.Request.Path
        };
    }

    private static Dictionary<string, string[]> ExtractValidationErrors(Error error)
    {
        if (error.Metadata is not null &&
            error.Metadata.TryGetValue("errors", out var rawErrors))
        {
            if (rawErrors is Dictionary<string, string[]> typedDictionary)
                return typedDictionary;

            if (rawErrors is IReadOnlyDictionary<string, string[]> typedReadOnlyDictionary)
                return typedReadOnlyDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        return new Dictionary<string, string[]>
        {
            ["request"] = new[] { error.Message }
        };
    }

    private static DateTimeOffset GetCurrentUtcNow(this ControllerBase controller)
    {
        var clock = controller.HttpContext.RequestServices.GetRequiredService<IDateTimeOffsetProvider>();
        return clock.UtcNow;
    }
}
