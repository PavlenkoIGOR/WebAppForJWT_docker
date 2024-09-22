using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAppForJWT.DTO;
using WebAppForJWT.Repository;
using WebAppForJWT.Services;

namespace WebAppForJWT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<JwtSettings>();
            builder.Services.AddTransient<IJwtAuthenticationManager, JwtAuthenticationManager>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region add JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(/*"OAuth",*/ config =>
                {
                    byte[] secretBytes = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
                    var key = new SymmetricSecurityKey(secretBytes);

                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true//,
                        //ClockSkew = TimeSpan.Zero
                    };
                });
            #endregion

            #region настройка пропуска по роли
            builder.Services.AddAuthorization(opt => 
            {
                opt.AddPolicy("RolePolicy", policy =>
                {
                    policy.Requirements.Add(new RoleRequirement("moder"));
                });
            });
            builder.Services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();
            #endregion

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // Добавление безопасности для Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Scheme = "Bearer",
                    BearerFormat = "Jwt",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {                                
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/swagger/req", async (HttpContext context) =>
            {
                var authorizationHeader = context.Request.Headers["Authorization"];
                string strOutput = "";

                // Проверяем, существует ли заголовок Authorization
                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    // Форматируем значение заголовка Authorization, заменяя запятую на </br>
                    strOutput = $"Authorization: {authorizationHeader}</br>";
                }

                return Results.Ok(strOutput); // Возвращает результат в виде OK ответа с заголовком Authorization
            });

            app.Run();
        }
    }
}
