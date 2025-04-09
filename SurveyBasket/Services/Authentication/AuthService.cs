using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using System.Security.Cryptography;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpirationInDays = 14;
    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!isValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationInDays);

        user.RefreshTokens.Add(new RefreshToken {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiryTime 
        });

        await _userManager.UpdateAsync(user);

        var result = new AuthResponse(user.Id, user.Email,
            user.FirstName, user.LastName,
            token, expiresIn, refreshToken, refreshTokenExpiryTime);

        return new Result<AuthResponse>(result, true, Error.None);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user =await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationInDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiryTime
        });

        await _userManager.UpdateAsync(user);

        var result =new AuthResponse(user.Id, user.Email,
            user.FirstName??"", user.LastName ?? "",
            newToken, expiresIn, newRefreshToken, refreshTokenExpiryTime);

        return new Result<AuthResponse>(result, true, Error.None);

    }
    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);
        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
         
        return Result.Success();
    }





    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); 
    }

}