using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using PF.Sol.EC.Consultas.Application.Interfaces;
using PF.Sol.EC.Consultas.Application.Services;
using PF.Sol.EC.Consultas.Infraestructure.Configurations;
using PF.Sol.EC.Consultas.Infraestructure.Data;
using PF.Sol.EC.Consultas.Infraestructure.Messaging;
using PF.Sol.EC.Consultas.Presentation.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = jwtSettings["Authority"];
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtSettings["Authority"],
            ValidateIssuer = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });

// Authorization
builder.Services.AddAuthorization();

builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddHostedService<KafkaConsumerService>();

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PF.Sol.EC.Consultas API", Version = "v1" });
});

// Add MongoDB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mongoClient = new MongoClient(connectionString);
var databaseName = "Consultas";
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddScoped<ConsultasDbContext>(provider =>
    new ConsultasDbContext(mongoClient, databaseName));

// Add services
builder.Services.AddScoped<IConsultasService, ConsultasService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints
app.MapConsultaEndpoints();

app.Run();
