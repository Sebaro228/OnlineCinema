using Azure.Identity;
using Azure.Storage.Blobs;
using BLL.Interfaces;
using BLL.Services;
using CinemaProject.Mapper;
using DAL.Context;
using DAL.Interfaces;
using DAL.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var keyVaultUri = new Uri("https://forcinemaproject.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(
    keyVaultUri,
    new DefaultAzureCredential()
);

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration["Storage:ConnectionString"]
        ?? throw new InvalidOperationException(
            "Storage connection string is not configured.");

    return new BlobServiceClient(connectionString);
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<CinemaContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("CinemaDb"),
        sqlOpts => sqlOpts.EnableRetryOnFailure()
    ));

var key = builder.Configuration["Jwt:Key"];
var emailKey = builder.Configuration["Resend:ApiKey"];

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<CinemaContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(CinemaMapperProfile)
);

builder.Services.AddResend(options =>
{
    options.ApiToken = emailKey;
});

builder.Services.AddScoped<IService<Film>, FilmService>();
builder.Services.AddScoped<IRepository<Film>, FilmRepository>();
builder.Services.AddScoped<IService<User>, UserService>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();

builder.Services.AddScoped<FilmService>();
builder.Services.AddScoped<FilmRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RefreshTokenRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(key)
        ),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
