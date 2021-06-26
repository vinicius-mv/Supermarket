using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Supermarket.API.V1.Dtos;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.API.V1.Controllers
{

    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthorizeController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthorizeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return $"{nameof(AuthorizeController)} :: Access: {DateTime.Now:D}";
        }

        /// <summary>
        /// Register an user in IdentityModels
        /// </summary>
        /// <param name="userInfo">an UserDto obj that requires essential user information</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUSer([FromBody] UserDto userInfo)
        {
            var user = new IdentityUser
            {
                UserName = userInfo.Email,
                Email = userInfo.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userInfo.Password);

            if (!result.Succeeded)
                BadRequest(result.Errors);

            await _signInManager.SignInAsync(user, false);

            return Ok(GenerateToken(userInfo));
        }

        /// <summary>
        /// Login user checking IdentityModels
        /// </summary>
        /// <param name="userInfo">an UserDto obj that requires essential user information</param>
        /// <returns>Bearer Token</returns>
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserDto userInfo)
        {
            // verify user credentials
            var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password,
                isPersistent: false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "InvalidLogin...");
                return BadRequest(ModelState);
            }
            return Ok(GenerateToken(userInfo));
        }

        private UserToken GenerateToken(UserDto userInfo)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("FavoriteFood", "PopCorn"),   // optional -> 'radom claim' more secure 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // generate private symmetrict key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // generate digital signature for token using HMAC and the private key
            var credentails = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define Token Expiration 
            var hours = double.Parse(_configuration["TokenConfiguration:ExpiryHours"]);
            var expiration = DateTime.UtcNow.AddHours(hours);

            var token = new JwtSecurityToken(
                issuer: _configuration["TokenConfiguration:Issuer"],
                audience: _configuration["TokenConfiguration:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentails);

            return new UserToken()
            {
                Authenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Message = "Token JWT OK"
            };
        }
    }
}
