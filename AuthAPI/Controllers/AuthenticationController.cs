using AuthAPI.DTO;
using AuthAPI.Infrastructure.Interfaces;
using AuthAPI.Model.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {        
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _unitOfWork.UserRepository.GetAll();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO loginModel)
        {
            var user = _unitOfWork.UserRepository.Login(loginModel.EmailUsername, loginModel.Password);
            if (user == null)
            {
                return Unauthorized("Invalid combination of username/email and password!");
            }
            else
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = user.Username
                });
            }
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDTO userModel)
        {
            if(string.IsNullOrEmpty(userModel.Username))
            {
                return Problem("Username empty");
            }
            if (string.IsNullOrEmpty(userModel.Password))
            {
                return Problem("Password empty");
            }
            if(userModel.Password != userModel.ConfirmPassword)
            {
                return Problem("Passwords don't match");
            }
            if(string.IsNullOrEmpty(userModel.Email))
            {
                return Problem("Email empty");
            }
            else
            {
                try
                {
                    MailAddress m = new MailAddress(userModel.Email);
                }
                catch (FormatException)
                {
                    return Problem("Invalid email");
                }
            }

            if (_unitOfWork.UserRepository.Find(u => u.Email == userModel.Email || u.Username.ToLower() == userModel.Username.ToLower()).Any())
            {
                return Problem("Username/email already in use");
            }

            try
            {
                User user = new User();
                user.Username = userModel.Username; 
                user.Password = userModel.Password;
                user.Email = userModel.Email;
                user.Created = DateTime.Now;
                user.LastLogin = DateTime.Now;
                
                await _unitOfWork.UserRepository.AddAsync(user);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            return Ok();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}