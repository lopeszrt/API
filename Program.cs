using API.Filters;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var uploadPath = builder.Configuration["UPLOAD_PATH"];
var publicBaseUrl = builder.Configuration["PUBLIC_BASE_URL"];

if (jwtSettings.Exists())
{
    var secretKey = jwtSettings["SecretKey"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];

    if (string.IsNullOrWhiteSpace(secretKey) ||
    string.IsNullOrWhiteSpace(issuer) ||
    string.IsNullOrWhiteSpace(audience))
    {
        throw new InvalidOperationException("One or more JwtSettings values are missing.");
    }

    if (string.IsNullOrWhiteSpace(uploadPath) || string.IsNullOrWhiteSpace(publicBaseUrl))
    {
        throw new InvalidOperationException("UPLOAD_PATH or PUBLIC_BASE_URL is not configured.");
    }

    builder.Services.AddScoped<Database>();
    builder.Services.AddScoped<DatabaseCalls>();
    builder.Services.AddScoped<JwtService>();
    builder.Services.AddScoped<AddRefreshedTokenFilter>();
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<AddRefreshedTokenFilter>();
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

    builder.Services.AddAuthorizationBuilder()
        .SetFallbackPolicy(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("User")
            .Build());


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    if (!Directory.Exists(imagesPath))
    {
        Directory.CreateDirectory(imagesPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imagesPath),
        RequestPath = "/images"
    });

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseMiddleware<JwtRefreshMiddleware>();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
else
{
    throw new InvalidOperationException("JWT settings are not configured in appsettings.json.");
}