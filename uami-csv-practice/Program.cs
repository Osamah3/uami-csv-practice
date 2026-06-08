using CsvUploadApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSingleton<BlobStorageService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority =
            "https://login.microsoftonline.com/f292caef-effe-4989-8d19-5ac4311f2c82";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = new[]
            {
                "292b52ce-31d3-4b52-88a7-e770ae0fd670",
                "api://292b52ce-31d3-4b52-88a7-e770ae0fd670"
            },

            ValidateIssuer = true,
            ValidIssuers = new[]
            {
                "https://login.microsoftonline.com/f292caef-effe-4989-8d19-5ac4311f2c82/v2.0",
                "https://sts.windows.net/f292caef-effe-4989-8d19-5ac4311f2c82/"
            }
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("AUTH FAILED:");
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("TOKEN VALIDATED");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
