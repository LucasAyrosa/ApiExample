using System.Linq;
using System.Threading.Tasks;
using API.Config;
using API.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public UserController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings)
        {
            _signInManger = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("new")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdentityUser>> Register(RegisterUserDto registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Created("GetUser", user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login(LoginUserDto loginUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var user = await _userManager.FindByEmailAsync(loginUser.Email);
            if (user is null) return BadRequest("Usuário ou senha incorretos");

            var result = await _signInManger.CheckPasswordSignInAsync(user, loginUser.Password, false);
            if (result.Succeeded) return Ok(await GenerateJwt(loginUser.Email));
            return BadRequest("Usuário ou senha incorretos");
        }

        private async Task<string> GenerateJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var identityClaims = new ClaimsIdentity();

            identityClaims.AddClaims(await _userManager.GetClaimsAsync(user));
            identityClaims.AddClaims(new[]
            {
                new Claim(ClaimTypes.Name, user.Id),
            });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}