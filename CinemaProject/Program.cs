using Azure.Identity;
using Azure.Storage.Blobs;
using BLL.Interfaces;
using BLL.Services;
using CinemaProject.Mapper;
using DAL.Context;
using DAL.Interfaces;
using DAL.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var keyVaultUri = new Uri("https://forcinemaproject.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(
    keyVaultUri,
    new DefaultAzureCredential()
);

builder.Services.AddDbContext<CinemaContext>(opts =>
    opts.UseSqlServer(
        builder.Configuration.GetConnectionString("CinemaDb"),
        sqlOpts => sqlOpts.EnableRetryOnFailure()
    ));

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration["Storage:ConnectionString"]
        ?? throw new InvalidOperationException(
            "Storage connection string is not configured.");

    return new BlobServiceClient(connectionString);
});

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(CinemaMapperProfile));

builder.Services.AddTransient<FilmRepository>();
builder.Services.AddTransient<FilmService>();

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
