using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application;
using Application.Services.UserRepository;
using Domain.Entities;
using Google.Apis.Auth.OAuth2.Requests;
using Life_Ecommerce.TokenService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthController(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    // [HttpGet("callback")]
    // public async Task<IActionResult> AuthCallback(string code, string state)
    // {
    //     // Exchange the code for tokens
    //     var auth0Client = new HttpClient();
    //     var tokenResponse = await auth0Client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
    //     {
    //         Address = $"https://{Configuration["Auth0:Domain"]}/oauth/token",
    //         ClientId = Configuration["Auth0:ClientId"],
    //         ClientSecret = Configuration["Auth0:ClientSecret"],
    //         Code = code,
    //         RedirectUri = Url.Action("AuthCallback", "Auth", null, Request.Scheme),
    //     });
    //
    //     if (tokenResponse.IsError)
    //     {
    //         return BadRequest("Error during token exchange");
    //     }
    //
    //     var handler = new JwtSecurityTokenHandler();
    //     var jsonToken = handler.ReadToken(tokenResponse.AccessToken) as JwtSecurityToken;
    //
    //     var auth0UserId = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    //     var email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    //     var name = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    //
    //     // Check if the user already exists in your database
    //     var existingUser = await _unitOfWork.Repository<User>().GetByCondition(x => x.LastName == auth0UserId).FirstOrDefaultAsync();
    //
    //     if (existingUser == null)
    //     {
    //         // Create a new user if one doesn't exist
    //         var newUser = new User
    //         {
    //             LastName = auth0UserId,
    //             Email = email,
    //             FirstName = name,
    //             // Add other necessary fields
    //         };
    //
    //         _unitOfWork.Repository<User>().Create(newUser);
    //         await _unitOfWork.CompleteAsync();
    //
    //         // Generate your application-specific JWT token if needed
    //         var jwt = TokenService.GenerateToken(newUser.Id, "User", newUser.Email);
    //         return Redirect($"your-app-url?token={jwt}");
    //     }
    //
    //     // If the user exists, generate a JWT token and redirect them
    //     var existingUserJwt = TokenService.GenerateToken(existingUser.Id, "User", existingUser.Email);
    //     return Redirect($"your-app-url?token={existingUserJwt}");
    // }
    
    
 [HttpGet("callback")]
public async Task<IActionResult> AuthCallback(string code, string state)
{
    var client = new HttpClient();
    var tokenEndpoint = $"https://{_configuration["Auth0:Domain"]}/oauth/token";

    var requestBody = new Dictionary<string, string>
    {
        {"grant_type", "authorization_code"},
        {"client_id", _configuration["Auth0:ClientId"]},
        {"client_secret", _configuration["Auth0:ClientSecret"]},
        {"code", code},
        {"redirect_uri", "https://localhost:7007/api/auth/callback"} // Ensure this matches Auth0 settings
    };

    var requestContent = new FormUrlEncodedContent(requestBody);
    var response = await client.PostAsync(tokenEndpoint, requestContent);
    var responseContent = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        return BadRequest($"Error during token exchange: {responseContent}");
    }

    var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
    var accessToken = tokenResponse["access_token"];

    // Decode the token to get user information
    var handler = new JwtSecurityTokenHandler();
    var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

    var auth0UserId = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    var email = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    var name = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

    // Check if the user already exists in your database
    var existingUser = await _unitOfWork.Repository<User>().GetByCondition(x => x.LastName == auth0UserId).FirstOrDefaultAsync();

    if (existingUser == null)
    {
        // Create a new user if one doesn't exist
        var newUser = new User
        {
            LastName = auth0UserId,
            Email = email,
            FirstName = name,
            // Add other necessary fields
        };

        _unitOfWork.Repository<User>().Create(newUser);
        await _unitOfWork.CompleteAsync();

        // Generate your application-specific JWT token if needed
        var jwt = TokenService.GenerateToken(newUser.Id, "User", newUser.Email);
        return Redirect($"https://localhost:7007?token={jwt}"); // Redirect with JWT
    }

    // If the user exists, generate a JWT token and redirect them
    var existingUserJwt = TokenService.GenerateToken(existingUser.Id, "User", existingUser.Email);
    return Redirect($"https://localhost:7007?token={existingUserJwt}"); // Redirect with JWT
}



}
