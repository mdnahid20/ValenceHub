using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValenceHub.Api.Contracts.Auth;
using ValenceHub.Api.Extensions;
using ValenceHub.Api.Mappings;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Messaging;
using MediatR;

namespace ValenceHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AuthController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            request.ToCommand(),
            cancellationToken);

        return result
            .Map(userId => new RegisterResponse(userId))
            .ToApiResponse(this);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch<Unit>(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch<Unit>(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("send-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendOtp(
        [FromBody] SendOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("resend-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("complete-registration")]
    [AllowAnonymous]
    public async Task<IActionResult> CompleteRegistration(
        [FromBody] CompleteRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch<Unit>(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.Dispatch<Unit>(
            request.ToCommand(),
            cancellationToken);

        return result.ToApiResponse(this);
    }
}
