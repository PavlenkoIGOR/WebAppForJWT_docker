using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WebAppForJWT.DTO;
using WebAppForJWT.Models;
using WebAppForJWT.Repository;

namespace WebAppForJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthRegController : ControllerBase
    {
        IConfiguration _config;
        IUserRepo _userRepo;
        IJwtAuthenticationManager _jwtAuthenticationManager;
        
        public AuthRegController(IJwtAuthenticationManager jwtAuthenticationManager, IConfiguration configuration, IUserRepo userRepo)
        {
            _userRepo = userRepo;
            _config = configuration;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }

        [HttpPost("registrate_User")]
        public async Task<ActionResult<User>> RegMethod(User user)
        {
            User userRepo = new User() 
            {
                Name = user.Name,
                Age = user.Age,
                Email = user.Email,
                Password = user.Password,
                Role = "user"
            };
            var token = _jwtAuthenticationManager.JWTAuthenticate(userRepo);

            int userID = await _userRepo.AddUserAndGetId(userRepo);
            user.Id = userID;
            return Ok(token);
        }

        [HttpPost("authenticate_User")]
        public async Task<ActionResult<string>> AuthMethod([FromBody]LoginModel loginModel)
        {
            //var computedHash = PasswordHasher.HashPassword(password);
            //var pass = computedHash.SequenceEqual(computedHash);

            User user = _userRepo.GetUserByEmail(loginModel.Email);

            if (user == null) 
            {
                return BadRequest("User not found:(");
            }
            #region создание JWT
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name), //обязательный токен
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserRole", user.Role)
            };
            var climesIdentity = new ClaimsIdentity(claims, "Cookies");

            #region формирование ключа лучше вывести в свой сервис/хелпер и т.п.
            byte[] secretBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var key = new SymmetricSecurityKey(secretBytes);
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            #endregion

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(60), //т.е. данный токен будет действителен в течении часа
                signingCredentials
                );

            var writeToken = new JwtSecurityTokenHandler().WriteToken(token);
            #endregion
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, user.Name),
            //        new Claim(ClaimTypes.Role, "User") // Установка роли в токене
            //    }),
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //var writeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(writeToken);
        }

    }
}
