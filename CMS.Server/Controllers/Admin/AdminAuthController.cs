using CMS.Contracts.Admin.Auth;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using CMS.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CMS.Server.Controllers;

[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController : ControllerBase
{
    private readonly CmsDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IEmailSender _emailSender;

    public AdminAuthController(
        CmsDbContext dbContext,
        IConfiguration configuration,
        IPasswordHasher<AppUser> passwordHasher,
        IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AdminLoginResponseDto>> Login(
        AdminLoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var emailOrUsername = request.EmailOrUsername.Trim();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(emailOrUsername))
            return BadRequest("Email or username is required.");

        if (!IsValidPassword(password))
            return BadRequest("Password must be at least 8 characters and contain uppercase, lowercase, number, and special character.");

        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(x =>
                x.Email == emailOrUsername ||
                x.UserName == emailOrUsername,
                cancellationToken);

        if (user is null)
            return Unauthorized("Invalid admin login credentials.");

        if (user.Role != UserRole.Admin)
            return Unauthorized("This account is not allowed to access the admin panel.");

        if (!user.IsActive)
            return Unauthorized("This admin account is inactive.");

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        if (passwordResult == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid admin login credentials.");

        if (!user.IsEmailVerified)
        {
            user.EmailVerificationCode = GenerateVerificationCode();
            user.EmailVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(15);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await SendAdminVerificationEmailAsync(user, cancellationToken);

            return Ok(new AdminLoginResponseDto
            {
                Succeeded = false,
                RequiresVerification = true,
                Message = "Admin account requires email verification. A verification code has been sent to your email."
            });
        }

        var token = CreateJwtToken(user);

        return Ok(new AdminLoginResponseDto
        {
            Succeeded = true,
            RequiresVerification = false,
            Message = "Admin login successful.",
            Token = token,
            Admin = ToMeDto(user)
        });
    }

    [HttpPost("verify-code")]
    public async Task<ActionResult<AdminLoginResponseDto>> VerifyCode(
        AdminVerifyCodeRequestDto request,
        CancellationToken cancellationToken)
    {
        var emailOrUsername = request.EmailOrUsername.Trim();
        var code = request.Code.Trim();

        if (string.IsNullOrWhiteSpace(emailOrUsername))
            return BadRequest("Email or username is required.");

        if (string.IsNullOrWhiteSpace(code))
            return BadRequest("Verification code is required.");

        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(x =>
                x.Email == emailOrUsername ||
                x.UserName == emailOrUsername,
                cancellationToken);

        if (user is null)
            return Unauthorized("Invalid verification request.");

        if (user.Role != UserRole.Admin)
            return Unauthorized("This account is not allowed to access the admin panel.");

        if (!user.IsActive)
            return Unauthorized("This admin account is inactive.");

        if (user.EmailVerificationCode != code)
            return BadRequest("Invalid verification code.");

        if (user.EmailVerificationCodeExpiresAtUtc is null ||
            user.EmailVerificationCodeExpiresAtUtc < DateTime.UtcNow)
        {
            return BadRequest("Verification code has expired.");
        }

        user.IsEmailVerified = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationCodeExpiresAtUtc = null;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var token = CreateJwtToken(user);

        return Ok(new AdminLoginResponseDto
        {
            Succeeded = true,
            RequiresVerification = false,
            Message = "Admin account verified.",
            Token = token,
            Admin = ToMeDto(user)
        });
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode(
        AdminResendCodeRequestDto request,
        CancellationToken cancellationToken)
    {
        var emailOrUsername = request.EmailOrUsername.Trim();

        if (string.IsNullOrWhiteSpace(emailOrUsername))
            return BadRequest("Email or username is required.");

        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(x =>
                x.Email == emailOrUsername ||
                x.UserName == emailOrUsername,
                cancellationToken);

        if (user is null)
            return Ok("If the admin account exists, a verification code has been sent.");

        if (user.Role != UserRole.Admin)
            return Ok("If the admin account exists, a verification code has been sent.");

        if (!user.IsActive)
            return Unauthorized("This admin account is inactive.");

        if (user.IsEmailVerified)
            return BadRequest("Admin account is already verified.");

        user.EmailVerificationCode = GenerateVerificationCode();
        user.EmailVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(15);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await SendAdminVerificationEmailAsync(user, cancellationToken);

        return Ok("Verification code resent.");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("me")]
    public async Task<ActionResult<AdminMeDto>> Me(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
            return Unauthorized();

        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
            return Unauthorized();

        if (user.Role != UserRole.Admin)
            return Unauthorized();

        return Ok(ToMeDto(user));
    }

    private async Task SendAdminVerificationEmailAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new InvalidOperationException("Admin email address is missing.");

        if (string.IsNullOrWhiteSpace(user.EmailVerificationCode))
            throw new InvalidOperationException("Verification code was not generated.");

        var displayName = string.IsNullOrWhiteSpace(user.FirstName)
            ? user.UserName
            : user.FirstName;

        var safeDisplayName = WebUtility.HtmlEncode(displayName);
        var safeCode = WebUtility.HtmlEncode(user.EmailVerificationCode);

        var subject = "PESYONG Admin Verification Code";

        var htmlBody = $"""
        <div style="font-family: Arial, sans-serif; max-width: 520px;">
            <h2>PESYONG Admin Verification</h2>

            <p>Hello {safeDisplayName},</p>

            <p>Your admin verification code is:</p>

            <div style="font-size: 28px; font-weight: bold; letter-spacing: 4px; margin: 20px 0;">
                {safeCode}
            </div>

            <p>This code will expire in 15 minutes.</p>

            <p>If you did not try to access the admin panel, you can ignore this email.</p>
        </div>
        """;

        await _emailSender.SendAsync(
            user.Email,
            subject,
            htmlBody,
            cancellationToken);
    }

    private string CreateJwtToken(AppUser user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static AdminMeDto ToMeDto(AppUser user)
    {
        return new AdminMeDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };
    }

    private static string GenerateVerificationCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }

    private static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password)
            && password.Length >= 8
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}