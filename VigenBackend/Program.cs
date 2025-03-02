using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vigen_Repository.Models;

var builder = WebApplication.CreateBuilder(args);
string corsConfiguration = "_corsConfiguration";
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<vigendbContext>(/*options=>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"))*/);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{

    options.AddPolicy(name: corsConfiguration,
        builder =>
        {
            builder.AllowCredentials();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.WithOrigins("http://localhost:5173");
        });
});

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("TuClaveSecreta")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
