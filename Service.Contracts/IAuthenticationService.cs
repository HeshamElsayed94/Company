using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;

namespace Service.Contracts;

public interface IAuthenticationService
{

    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);

    Task<User?> ValidateUser(UserForAuthenticationDto userForAuth);

    Task<TokenDto> CreateToken(User user, bool populateExp);

    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}