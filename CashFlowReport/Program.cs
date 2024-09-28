using System.Text;
using System.Text.Json;
using CashFlowInfra.Data;
using CashFlowInfra.Interfaces;
using CashFlowInfra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddScoped<IReportService, ReportService>();
services.AddScoped<ITransactionService, TransactionService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var keyValue = jwtSettings.GetValue<string>("SecretKey");
var issuerValue = jwtSettings.GetValue<string>("Issuer");
var audienceValue = jwtSettings.GetValue<string>("Audience");

var key = Encoding.ASCII.GetBytes(keyValue);

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuerValue, 
        ValidAudience = audienceValue, 
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(new { message = "Token expired. Please log in again." });
                return context.Response.WriteAsync(json);
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = "Authentication failed." });
            return context.Response.WriteAsync(result);
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = "You are not authorized." });
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new { message = "You do not have permission to access this resource." });
            return context.Response.WriteAsync(result);
        }
    };
});

services.AddAuthorization();

services.AddControllers();


services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CashFlowControl API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorização JWT usando o esquema Bearer (exemplo: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CashFlowControl API v1");
        c.RoutePrefix = string.Empty;
    });
    app.UseMetricServer(); 
    app.UseHttpMetrics();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization(); 

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();