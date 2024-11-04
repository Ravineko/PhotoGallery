using FluentValidation;
using FluentValidation.AspNetCore;
using PhotoGallery.Configurations;
using PhotoGallery.Configurations.Constants;
using PhotoGallery.Configurations.Extensions;
using PhotoGallery.Models.Mapper;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<JWTSettings>(
    configuration.GetRequiredSection(ConfigurationConstants.JWTSettingsSection));
builder.Services.Configure<ImageSettings>(
    configuration.GetRequiredSection(ConfigurationConstants.ImageSettingsSection));
builder.Services.Configure<CodesExpirySettings>(
    configuration.GetRequiredSection(ConfigurationConstants.CodeExpirySettingsSection));
builder.Services.Configure<EmailSettings>(
    configuration.GetRequiredSection(ConfigurationConstants.EmailSettingsSection));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();

var connectionSection = configuration.GetSection("ConnectionStrings");
var connectionString = connectionSection["DefaultConnection"];

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddCustomSwagger();
var secretKey = builder.Configuration["JWTSettings:SecretKey"];
builder.Services.AddCustomJwtAuthentication(secretKey);
builder.Services.AddDependecies(connectionString);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
