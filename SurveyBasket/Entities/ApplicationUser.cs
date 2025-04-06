using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Entities;

public class ApplicationUser:IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(); 
}
