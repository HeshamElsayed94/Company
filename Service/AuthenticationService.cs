using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Service.Mapping;
using Shared.DTOs;

namespace Service;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILoggerManager _logger;
    private readonly MappingProfile _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IOptionsMonitor<JwtConfiguration> _configuration;

    public AuthenticationService(
        ILoggerManager logger,
        MappingProfile mapper,
        UserManager<User> userManager,
        IOptionsMonitor<JwtConfiguration> configuration

    )
    {
        this._logger = logger;
        this._mapper = mapper;
        this._userManager = userManager;
        this._configuration = configuration;
        _jwtConfiguration = _configuration.CurrentValue;
    }

    public async Task<TokenDto> CreateToken(User user, bool populateExp)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims(user);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;

        if (populateExp)
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = _mapper.ToUserEntity(userForRegistration);

        var result = await _userManager.CreateAsync(user, userForRegistration.Password!);

        if (result.Succeeded)
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles);

        return result;
    }

    public async Task<User?> ValidateUser(UserForAuthenticationDto userForAuth)
    {
        var user = await _userManager.FindByNameAsync(userForAuth.UserName!);

        var validLogin = user is not null && await _userManager.CheckPasswordAsync(user, userForAuth.Password!);

        if (!validLogin)
        {
            _logger.LogWarn($"{nameof(ValidateUser)}: authentication failed. Wrong user name or password");
            return null;
        }

        return user;
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);

        if (user is null
            || user.RefreshToken != tokenDto.RefreshToken
            || user.RefreshTokenExpiryTime <= DateTime.UtcNow
            )
            throw new RefreshTokenBadRequest();


        return await CreateToken(user, false);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5),

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"))),
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token,
            tokenValidationParameters,
            out var validatedToken);


        if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg
            .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
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

        var userClaims = await _userManager.GetClaimsAsync(user);

        claims.AddRange(userClaims);

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
        issuer: _jwtConfiguration.ValidIssuer,
        audience: _jwtConfiguration.ValidAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
        signingCredentials: signingCredentials
        );

        return tokenOptions;
    }
}