using Microsoft.EntityFrameworkCore;
using SoftBook.Domain.Data;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Infrastructure.Repositories.Classes;
using SoftBook.Domain.Services.Interfaces;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Application.Services.Classes;
using SoftBook.Application.Services;
using SoftBook.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Infrastructure.Classes; // For CsvAuditLogService
using SoftBook.API.Services; // For new TenantProvider
using SoftBook.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SoftBook.API.Filters.DatabaseExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SoftBook.API.Filters.TenantHeaderFilter>();
});

// Enable CORS for React Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
                new Uri(origin).Host == "localhost" || 
                new Uri(origin).Host == "127.0.0.1")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Important for cookies/auth
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// 1. ApplicationDbContext (Global/Master DB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. UserContext (Tenant-specific, but effectively same DB in new model)
builder.Services.AddDbContext<UserContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
}, ServiceLifetime.Scoped);

// Register Tenant Services
builder.Services.AddScoped<ITenantProvider, SoftBook.API.Services.TenantProvider>();
builder.Services.AddScoped<ITenantResolutionService, TenantResolutionService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not found"));

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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Register Repositories
// Register Audit Log Service
builder.Services.AddScoped<IAuditLogService, CsvAuditLogService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPartyRepository, PartyRepository>();
builder.Services.AddScoped<IPartyAddressRepository, PartyAddressRepository>();
builder.Services.AddScoped<IPartyBankDetailsRepository, PartyBankDetailsRepository>();
builder.Services.AddScoped<IPartyContactPersonRepository, PartyContactPersonRepository>();
builder.Services.AddScoped<IPartyReportingTagRepository, PartyReportingTagRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IMasterRepository, MasterRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddScoped<IPartyAddressService, PartyAddressService>();
builder.Services.AddScoped<IPartyBankDetailsService, PartyBankDetailsService>();
builder.Services.AddScoped<IPartyContactPersonService, PartyContactPersonService>();
builder.Services.AddScoped<IPartyReportingTagService, PartyReportingTagService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// ENABLE SWAGGER FOR IIS (Production)
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SoftBook.API v1");
    c.RoutePrefix = "swagger";
});


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
