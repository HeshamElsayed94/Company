using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

public sealed class AuthenticationService(
    ILoggerManager logger,
    MappingProfile mapper,
    UserManager<User> userManager,
    IConfiguration configuration
    ) : IAuthenticationService
{
    public async Task<string> CreateToken(User user)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = mapper.ToUserEntity(userForRegistration);

        var result = await userManager.CreateAsync(user, userForRegistration.Password!);

        if (result.Succeeded)
            await userManager.AddToRolesAsync(user, userForRegistration.Roles);

        return result;
    }

    public async Task<User?> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        var user = await userManager.FindByNameAsync(userForAuth.UserName!);

        var validLogin = user is not null && await userManager.CheckPasswordAsync(user, userForAuth.Password!);

        if (!validLogin)
        {
            logger.LogWarn($"{nameof(ValidateUser)}: authentication failed. Wrong user name or password");
            return null;
        }

        return user;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")
            ?? throw new Exception("Secret key not found"));

        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims(User user)
    {
        List<Claim>? claims =
                [
                     new(ClaimTypes.Name, user.UserName!)
                ];

        var userClaims = await userManager.GetClaimsAsync(user);

        claims.AddRange(userClaims);

        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenOptions = new JwtSecurityToken
        (
        issuer: jwtSettings["validIssuer"],
        audience: jwtSettings["validAudience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
        signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
}