using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAppForJWT.DTO;
using WebAppForJWT.Models;

namespace WebAppForJWT;
public interface IJwtAuthenticationManager
{
    public string JWTAuthenticate(User user);
}

public class JwtAuthenticationManager : IJwtAuthenticationManager
{
    IConfiguration _config;
    JwtSettings _jwtSettings;
    public JwtAuthenticationManager(IConfiguration config, JwtSettings jwtSettings)
    {
        _config = config;
        _jwtSettings = jwtSettings;
    }
    public string JWTAuthenticate(User user)
    {
        List<Claim> claims = new()
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Name), //обязательный токен
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //new Claim(ClaimTypes.Role, user.Role), //ClaimTypes более гибкие в настройке, JwtRegisteredClaimNames - более универсальны
                new Claim("UserRole", user.Role),
                new Claim("Password", user.Password)
        };
        var climesIdentity = new ClaimsIdentity(claims, "Cookies");

        #region формирование ключа лучше вывести в свой сервис/хелпер и т.п.
        byte[] secretBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var key = new SymmetricSecurityKey(secretBytes);
        SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        #endregion

        //старый рабочий вариант
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(60), //т.е. данный токен будет действителен в течении часа
            signingCredentials);

        var writeToken = new JwtSecurityTokenHandler().WriteToken(token);
        return writeToken;


        ////новый сомнительный нерабочий вариант
        //_jwtSettings = _config.GetSection("Jwt").Get<JwtSettings>();

        //var tokenDescriptor = new SecurityTokenDescriptor()
        //{
        //    Subject = new ClaimsIdentity(claims),
        //    //Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("Jwt").GetValue<int>("TokenLifeTime"), //либо вытягиваем данные напрямую из appsettings.json либо смследующую строку
        //    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifeTime),
        //    SigningCredentials = signingCredentials,
        //    Issuer = _jwtSettings.Issuer,
        //    Audience = _jwtSettings.Audience
        //};

        //var tokenHandler = new JwtSecurityTokenHandler();
        //var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        //return tokenHandler.WriteToken(securityToken);
    }
}
