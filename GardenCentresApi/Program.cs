using Asp.Versioning;
using GardenCentresApi.Data;
using GardenCentresApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<GardenCentreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with configuration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<GardenCentreContext>()
    .AddDefaultTokenProviders();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register repositories - but make them conditional
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILocationRepository>(provider =>
{
    var context = provider.GetRequiredService<GardenCentreContext>();
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

    // Check if we have an HTTP context and user
    var httpContext = httpContextAccessor.HttpContext;
    if (httpContext?.User?.Identity?.IsAuthenticated == true)
    {
        var region = httpContext.User.FindFirst("Region")?.Value;
        if (!string.IsNullOrWhiteSpace(region) && (region == "UK" || region == "US"))
        {
            return new LocationRepository(context, region);
        }
    }

    // Return a default implementation or throw a more specific exception
    throw new InvalidOperationException("User must be authenticated with a valid Region claim (UK or US).");
});

builder.Services.AddScoped<IGardenCentreRepository>(provider =>
{
    var context = provider.GetRequiredService<GardenCentreContext>();
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

    // Check if we have an HTTP context and user
    var httpContext = httpContextAccessor.HttpContext;
    if (httpContext?.User?.Identity?.IsAuthenticated == true)
    {
        var region = httpContext.User.FindFirst("Region")?.Value;
        if (!string.IsNullOrWhiteSpace(region) && (region == "UK" || region == "US"))
        {
            return new GardenCentreRepository(context, region);
        }
    }

    // Return a default implementation or throw a more specific exception
    throw new InvalidOperationException("User must be authenticated with a valid Region claim (UK or US).");
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Garden Centres API",
        Description = "API for managing garden centres and locations."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer prefix",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Garden Centres API v1");
    });
}

app.UseHttpsRedirection();
app.UseRouting(); // Add this explicitly
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();