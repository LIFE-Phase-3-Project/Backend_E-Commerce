using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class AChangeRoleController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AChangeRoleController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] RoleAssignmentRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var assignRoleUrl = $"https://{_configuration["Auth0:Domain"]}/api/v2/users/{request.UserId}/roles";
        var roleAssignment = new { roles = new[] { request.RoleId } };
        var content = new StringContent(JsonConvert.SerializeObject(roleAssignment), Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, assignRoleUrl)
        {
            Content = content,
            Headers =
            {
                { "Authorization", $"Bearer {request.AccessToken}" }
            }
        };

        var response = await httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            return Ok();
        }

        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }
}

public class RoleAssignmentRequest
{
    public string AccessToken { get; set; }
    public string UserId { get; set; }
    public int RoleId { get; set; }
}