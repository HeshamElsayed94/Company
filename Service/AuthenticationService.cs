using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = mapper.ToUserEntity(userForRegistration);

        var result = await userManager.CreateAsync(user, userForRegistration.Password!);

        if (result.Succeeded)
            await userManager.AddToRolesAsync(user, userForRegistration.Roles);

        return result;
    }
}