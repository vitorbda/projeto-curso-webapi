using Microsoft.EntityFrameworkCore;
using APICatalogo.Context;
using System.Text.Json.Serialization;
using APICatalogo.Services;
using APICatalogo.Extensions;
using APICatalogo.Logging;
using APICatalogo.Filters;
using APICatalogo.DTOs.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using APICatalogo.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using APICatalogo.RateLimitOptions;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key!!");

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen( c=>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "APICatalogo",
        Version = "v1",
        Description = "Catálogo de Produtos e Categorias",
        TermsOfService = new Uri("https://macoratti.net/terms"),
        Contact = new OpenApiContact
        {
            Name = "João Vitor",
            Email = "bdandrade.joaovitor@gmail.com",
            Url = new Uri("https://macoratti.net")
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://macoratti.net/license")
        }

    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT "
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
            new string[] {}
        }
    });
});

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mySqlConnection,
                ServerVersion.AutoDetect(mySqlConnection)
            ));

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
})
.AddNewtonsoftJson();

//var OrigensComAcessoPermitido = "_origensComAcessoPermitido";

builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigensComAcessoPermitido",
        policy =>
        {
            policy.WithOrigins("https://localhost:7233")
            .WithMethods("GET", "POST")
            .AllowAnyHeader();
        });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var myOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixedwindow", options =>
    {
        options.PermitLimit = myOptions.PermitLimit;//1;
        options.Window = TimeSpan.FromSeconds(myOptions.Window);
        options.QueueLimit = myOptions.QueueLimit;//0;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                QueueLimit = 0,
                Window = TimeSpan.FromSeconds(10)
            }));
});

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader(),
        new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    option.AddPolicy("SuperAdminOnly", policy => 
        policy.RequireRole("Admin")
        .RequireClaim("id", "joao"));

    option.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

    option.AddPolicy("ExclusivePolicyOnly", policy =>
    {
        policy.RequireAssertion(context => context.User.HasClaim(claim =>        
            claim.Type == "id" && claim.Value == "joao")
            || context.User.IsInRole("SuperAdmin")
        );
    });
});

builder.Services.AddAutoMapper(typeof(DTOMapperProfile));

builder.Services.ConfigureServices();

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
