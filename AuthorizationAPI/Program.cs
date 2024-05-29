using System.Reflection;
using System.Text;
using AuthorizationAPI.Core;
using AuthorizationAPI.Core.Requests;
using AuthorizationAPI.Core.Settings;
using AuthorizationAPI.Infrastructure.Models;
using AuthorizationAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using AuthorizationAPI.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace AuthorizationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtConfig = builder.Configuration.GetSection(nameof(Jwt));
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<TokenRefreshRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                c.SupportNonNullableReferenceTypes();
            });
            var mongoDbClient = new MongoClient(builder.Configuration.GetConnectionString("UsersDatabase"));
            var mongoDb = mongoDbClient.GetDatabase(UserRepository.CollectionName);
            builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 310000;
            });
            builder.Services.AddSingleton(mongoDbClient);
            builder.Services.AddScoped(_ => mongoDb);
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
            builder.Services.AddScoped<IUserManagementService, UserManagementService>();
            builder.Services.AddHttpContextAccessor();

            //JWT auth security options
            var jwtSettings = jwtConfig.Get<Jwt>();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SigningKey)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudiences = jwtSettings.Audiences,
                ClockSkew = TimeSpan.Zero
            };
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());
            });
            //Configuration options
            builder.Services.AddSingleton(tokenValidationParameters);
            builder.Services.Configure<Jwt>(jwtConfig);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = [new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }];
                    });
                });
                app.UseSwaggerUI();
            }

            mongoDb.AuthorizationDbInitialisation(app.Services.GetService<IPasswordHasher<User>>()!);
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.UseAuthentication();
            app.MapControllers();
          

            app.Run();
        }
    }
}
