using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); 
builder.Logging.AddConsole(); 

var services = builder.Services;

var environment = builder.Environment.EnvironmentName;
if (environment == "Development")
{
    builder.Logging.AddDebug(); 
}
if (environment == "Production")
{
    services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:InstrumentationKey"]);
}

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

services.AddAuthorization();

services.AddControllers();

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

app.UseAuthentication();  
app.UseAuthorization(); 

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
