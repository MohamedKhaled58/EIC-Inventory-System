using EICInventorySystem.Application;
using EICInventorySystem.Infrastructure;
using EICInventorySystem.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EIC Inventory System API",
        Version = "v1",
        Description = "Enginerring Industrial Complex Inventory Command System - مجمع الصناعات الهندسية - نظام إدارة المخازن",
        Contact = new OpenApiContact
        {
            Name = "EIC Inventory System",
            Email = "support@eic-inventory.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

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
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    // Commander's Reserve Access Policy
    options.AddPolicy("AccessCommanderReserve", policy =>
        policy.RequireAssertion(context =>
        {
            var role = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "FactoryCommander" || role == "ComplexCommander";
        }));

    // Complex Commander Policy
    options.AddPolicy("ComplexCommanderOnly", policy =>
        policy.RequireRole("ComplexCommander"));

    // Factory Commander Policy
    options.AddPolicy("FactoryCommanderOrHigher", policy =>
        policy.RequireRole("FactoryCommander", "ComplexCommander"));

    // Warehouse Keeper Policy
    options.AddPolicy("WarehouseKeeper", policy =>
        policy.RequireRole("CentralWarehouseKeeper", "FactoryWarehouseKeeper"));

    // Officer Policy
    options.AddPolicy("OfficerOrHigher", policy =>
        policy.RequireRole("Officer", "FactoryCommander", "ComplexCommander"));

    // Department Head Policy
    options.AddPolicy("DepartmentHeadOrHigher", policy =>
        policy.RequireRole("DepartmentHead", "Officer", "FactoryCommander", "ComplexCommander"));

    // Project Manager Policy
    options.AddPolicy("ProjectManagerOrHigher", policy =>
        policy.RequireRole("ProjectManager", "DepartmentHead", "Officer", "FactoryCommander", "ComplexCommander"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EIC Inventory System API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

// Use Serilog request logging
app.UseSerilogRequestLogging();

// Use JWT Middleware
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting EIC Inventory System API");

    // Seed Data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<EICInventorySystem.Infrastructure.Data.ApplicationDbContext>();
            await EICInventorySystem.Infrastructure.Data.SeedData.SeedAsync(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
