using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vigen_Repository.Models;

var builder = WebApplication.CreateBuilder(args);
string corsConfiguration = "_corsConfiguration";

// Cargar configuración de JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Agregar servicios
builder.Services.AddControllers();
builder.Services.AddDbContext<vigendbContext>(/*options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"))*/);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsConfiguration, builder =>
    {
        builder.AllowCredentials();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        builder.WithOrigins("http://localhost:5173");
    });
});

// Configurar autenticación con JWT
builder.Services.AddAuthentication(options =>
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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"])), // ✅ Se obtiene de appsettings.json
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configurar el pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // Este middleware debe estar antes de Authorization

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(corsConfiguration);

app.UseRouting();

app.UseEndpoints(enpoint =>
{
    enpoint.MapHub<BroadCastHub>("/NotifyHub");
});

app.UseEndpoints(enpoint =>
{
    enpoint.MapControllers();
});

app.Run();