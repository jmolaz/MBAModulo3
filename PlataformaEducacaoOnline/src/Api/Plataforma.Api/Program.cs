using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plataforma.Api.Auth;
using Plataforma.Alunos.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ----------------------------------------------------
// Diretórios e bancos SQLite
// ----------------------------------------------------
var dataDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data"));
Directory.CreateDirectory(dataDir);

string authDbPath = Path.Combine(dataDir, "auth.db");
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
// DbContexts (SQLite)
// ----------------------------------------------------
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlite($"Data Source={authDbPath}"));

builder.Services.AddDbContext<AlunosDbContext>(opt =>
    opt.UseSqlite($"Data Source={alunosDbPath}"));

// ----------------------------------------------------
// Identity + JWT
// ----------------------------------------------------
builder.Services.AddIdentityCore<ApplicationUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AuthDbContext>()
.AddSignInManager()        // se você usa UserManager/SignInManager
.AddDefaultTokenProviders();


var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false
        };

    });

// ----------------------------------------------------
// Políticas de autorização
// ----------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AlunoOnly", policy =>
        policy.RequireClaim("persona", "Aluno"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("persona", "Admin"));
});

// ----------------------------------------------------
// Retornar 401/403 em vez de redirecionar para /Account/Login
// ----------------------------------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

// ----------------------------------------------------
// Build e Middlewares
// ----------------------------------------------------
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run("http://127.0.0.1:5280");
