using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Errors;
using SurveyBasket.Services;
using System.Threading.Tasks;

namespace SurveyBasket.Controllers;
public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {

        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
        
        return authResult.IsSuccess 
            ? Ok(authResult) 
            : authResult.ToProblem();
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess
            ? Ok(authResult)
            : authResult.ToProblem();
    }
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var revokedResult = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return revokedResult.IsSuccess
            ? Ok()
            :revokedResult.ToProblem();
    }
}
