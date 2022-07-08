using System.Security.AccessControl;
using System.Text;
using ElevenNote.Data;
using ElevenNote.Services.User;
using ElevenNote.Services.Token;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//Add connection string and DbContext setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// ApplicationDbContext exists in ElevenNote.Data, so a namespace is required to access the class
// UseSqlServer() also requires a using statement, Microsoft.EntityFrameworkCore
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

//Add User Service/Interface for Dependency Injection here
builder.Services.AddScoped<IUserService, UserService>();
// Add Token Service/Interface for Dependency Injection here
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetaData = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
