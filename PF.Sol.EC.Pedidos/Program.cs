using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PF.Sol.EC.Pedidos.Application.Http;
using PF.Sol.EC.Pedidos.Application.Interfaces;
using PF.Sol.EC.Pedidos.Application.Services;
using PF.Sol.EC.Pedidos.Infraestructure.Configurations;
using PF.Sol.EC.Pedidos.Infraestructure.Data;
using PF.Sol.EC.Pedidos.Infraestructure.Messaging;
using PF.Sol.EC.Pedidos.Presentation.Endpoints;
using Polly;
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

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PF.Sol.EC.Pedidos API", Version = "v1" });
});

// Add Entity Framework
builder.Services.AddDbContext<PedidoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Services
builder.Services.AddScoped<IPedidosService, PedidosService>();
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(builder.Configuration["Services:Pedido:Name"]!))
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddSource(builder.Configuration["Services:Pedido:Name"]!)
         .AddOtlpExporter(opt =>
         {
             opt.Endpoint = new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"]!);
             opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
         });
    });

// HttpClient para llamar ApiPagos
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Pago:BaseUrl"]!);
})
.AddResilienceHandler("pago-policies", (builder, context) =>
{
    builder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(200),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,

        // Aquí defines cuándo reintentar
        ShouldHandle = args =>
        {
            // Excepción de red
            if (args.Outcome.Exception is HttpRequestException)
                return PredicateResult.True();

            // Respuestas HTTP específicas
            if (args.Outcome.Result is HttpResponseMessage response)
            {
                if ((int)response.StatusCode >= 500 ||   // 5xx
                    response.StatusCode == System.Net.HttpStatusCode.BadRequest || // 400
                    response.StatusCode == (System.Net.HttpStatusCode)429)         // Too Many Requests
                {
                    return PredicateResult.True();
                }
            }

            return PredicateResult.False();
        },

        OnRetry = args =>
        {
            Console.WriteLine($"[RETRY] intento {args.AttemptNumber}, resultado: {args.Outcome.Exception?.GetType().Name ?? args.Outcome.Result?.StatusCode.ToString()}");
            return default;
        }
    });
});

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

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PedidoDbContext>();
    context.Database.EnsureCreated();
}

// Endpoints
app.MapPedidoEndpoints();

app.Run();
