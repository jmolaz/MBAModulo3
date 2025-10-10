using System.Text;
using System.IO; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Plataforma.Api.Auth;
using Plataforma.Alunos.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


var dataDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data"));
Directory.CreateDirectory(dataDir); 

string authDbPath   = Path.Combine(dataDir, "auth.db");
string alunosDbPath = Path.Combine(dataDir, "alunos.db");

// ----------------------------------------------------
// Controllers + Swagger
// ----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plataforma.Api", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira: Bearer {seu_token_jwt}",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});


// ----------------------------------------------------
// DbContexts (SQLite) - usando caminhos absolutos
// ----------------------------------------------------
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlite($"Data Source={authDbPath}"));

builder.Services.AddDbContext<AlunosDbContext>(opt =>
    opt.UseSqlite($"Data Source={alunosDbPath}"));

// ----------------------------------------------------
// Identity + JWT Authentication
// ----------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

// ----------------------------------------------------
// Build e Middlewares
// ----------------------------------------------------
var app = builder.Build();

// Swagger sempre habilitado
app.UseSwagger();
app.UseSwaggerUI();

// Sem redirecionar para HTTPS (ficamos em HTTP)
//// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

// Porta fixa
app.Run("http://127.0.0.1:5280");
