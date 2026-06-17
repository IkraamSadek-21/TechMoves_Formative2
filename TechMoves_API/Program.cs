using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TechMoves_API.Data;
using TechMoves_API.Factories;
using TechMoves_API.Observers;
using TechMoves_API.Services;
using TechMoves_API.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<TechMoveDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddHttpClient<ICurrencyConversionService, CurrencyConversionService>();
builder.Services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();
builder.Services.AddSingleton<ContractNotifier>(sp =>
{
    var notifier = new ContractNotifier();
    notifier.Subscribe(new ContractStatusObserver());
    return notifier;
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechMoves API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TechMoveDb>();
    db.Database.Migrate();
}
// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TechMoveDb>();
    db.Database.Migrate();

    // Seed dummy data if empty
    if (!db.Clients.Any())
    {
        var clients = new List<Client>
        {
            new Client { ClientName = "Acme Corporation", ClientEmail = "acme@email.com", ClientRegion = "Gauteng" },
            new Client { ClientName = "BlueSky Logistics", ClientEmail = "bluesky@email.com", ClientRegion = "Western Cape" },
            new Client { ClientName = "Nova Tech", ClientEmail = "nova@email.com", ClientRegion = "KwaZulu-Natal" }
        };
        db.Clients.AddRange(clients);
        db.SaveChanges();

        var contracts = new List<Contract>
        {
            new Contract { ContractName = "Acme Support Contract", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1), Status = "Active", ServiceLevel = "Gold", ClientID = clients[0].ClientID },
            new Contract { ContractName = "BlueSky Maintenance", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6), Status = "Active", ServiceLevel = "Silver", ClientID = clients[1].ClientID },
            new Contract { ContractName = "Nova Tech Premium", StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(2), Status = "Pending", ServiceLevel = "Platinum", ClientID = clients[2].ClientID }
        };
        db.Contracts.AddRange(contracts);
        db.SaveChanges();

        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { RequestType = "Hardware Repair", Description = "Replace faulty server hardware", Cost = 5000.00m, Status = "Open", ContractID = contracts[0].ContractID },
            new ServiceRequest { RequestType = "Network Setup", Description = "Configure internal office network", Cost = 3500.00m, Status = "In Progress", ContractID = contracts[1].ContractID },
            new ServiceRequest { RequestType = "Software Installation", Description = "Install and configure ERP system", Cost = 12000.00m, Status = "Completed", ContractID = contracts[2].ContractID }
        };
        db.ServiceRequests.AddRange(serviceRequests);
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowMVC");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { } //makes this file testable for integration tests