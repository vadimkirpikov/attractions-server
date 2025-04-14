using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TouristServer.DAL;
using TouristServer.Extensions;
using TouristServer.Presentation;
using TouristServer.Presentation.Dto;
using TouristServer.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Description = "Enter your Bearer token"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("JWTSettings");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.Configure<JwtSettings>(jwtSettings).AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddCustomAuthentication(jwtSettings).AddCors(options =>
{
    options.AddDefaultPolicy(b =>
    {
        b.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
}).AddValidatorsFromAssemblyContaining(typeof(FilterDtoValidator))
    .AddValidatorsFromAssemblyContaining(typeof(UserRouteValidator))
    .AddValidatorsFromAssemblyContaining(typeof(LoginDtoValidator))
    .AddValidatorsFromAssemblyContaining(typeof(RegisterDtoValidator));


var app = builder.Build();
app.CreateDbIfNotExists();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Map("/", () => Results.Redirect("/swagger/index.html"));
app.Run();
