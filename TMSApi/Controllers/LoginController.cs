using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace TMSApi;


[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginController> _logger;
    public LoginController(IUserService userService, IConfiguration configuration, ILogger<LoginController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }
    [HttpPost]
    public IActionResult Login(LoginRequest user)
    {
        try
        {
            var userDTO = _userService.CheckUser(user.Email, user.Password);
            //Bad request
            if (userDTO == null)
            {
                return Unauthorized("wrong email or password");
            }
            string? key = _configuration["Jwt:Key"];
            if (key == null)
            {
                return StatusCode(500, "Key not found");
            }
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Email)
                ,new Claim(ClaimTypes.Role, userDTO.Role.ToString())
                ]),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            var tokenString = tokenhandler.WriteToken(token);
            _logger.LogInformation($"User {user.Email} logged in");
            return Ok(new { Token = "Bearer " + tokenString });
        }catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing your request.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

}
