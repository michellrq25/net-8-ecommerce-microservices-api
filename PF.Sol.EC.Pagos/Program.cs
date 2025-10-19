using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PF.Sol.EC.Pagos.Application.Interfaces;
using PF.Sol.EC.Pagos.Application.Services;
using PF.Sol.EC.Pagos.Infraestructure.Data;
using PF.Sol.EC.Pagos.Presentation.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));


// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PF.Sol.EC.Pagos API", Version = "v1" });
});

// Add MongoDB
// Add Entity Framework
builder.Services.AddDbContext<PagosDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

// Add services
builder.Services.AddScoped<IPagosService, PagosService>();

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(builder.Configuration["Services:Pago:Name"]!))
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation() // Instrumenta las peticiones entrantes
         .AddSource(builder.Configuration["Services:Pago:Name"]!)
         .AddOtlpExporter(opt =>
         {
             opt.Endpoint = new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"]!);
             opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
         });
    });

var app = builder.Build();

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
    var context = scope.ServiceProvider.GetRequiredService<PagosDbContext>();
    context.Database.EnsureCreated();
}

// Endpoints
app.MapPagoEndpoints();

app.Run();