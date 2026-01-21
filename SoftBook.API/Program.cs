using Microsoft.EntityFrameworkCore;
using SoftBook.Domain.Data;
using SoftBook.Domain.RepositoryInterfaces;
using SoftBook.Infrastructure.Repositories.Classes;
using SoftBook.Application.Services.Interfaces;
using SoftBook.Application.Services.Classes;
using SoftBook.Application.Services;
using SoftBook.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SoftBook.Infrastructure.Interfaces;
using SoftBook.Infrastructure.Classes;
using SoftBook.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SoftBook.API.Filters.TenantHeaderFilter>();
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<UserContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("UserContext");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'UserContext' not found.");
    }
    options.UseSqlite(connectionString);
}, ServiceLifetime.Scoped);

// Register Tenant Services
builder.Services.AddScoped<ITenantResolutionService, TenantResolutionService>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IHSNRepository, HSNRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<IPaymentModeRepository, PaymentModeRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPartyRepository, PartyRepository>();
builder.Services.AddScoped<IPartyAddressRepository, PartyAddressRepository>();
builder.Services.AddScoped<IPartyBankDetailsRepository, PartyBankDetailsRepository>();
builder.Services.AddScoped<IPartyContactPersonRepository, PartyContactPersonRepository>();
builder.Services.AddScoped<IPartyReportingTagRepository, PartyReportingTagRepository>();
builder.Services.AddScoped<IPartyTypeMasterRepository, PartyTypeMasterRepository>();
builder.Services.AddScoped<ICostCenterRepository, CostCenterRepository>();
builder.Services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();
builder.Services.AddScoped<IGodownRepository, GodownRepository>();
builder.Services.AddScoped<IPaymentTermRepository, PaymentTermRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISACRepository, SACRepository>();
builder.Services.AddScoped<ISalespersonRepository, SalespersonRepository>();
builder.Services.AddScoped<ITaxGroupRepository, TaxGroupRepository>();
builder.Services.AddScoped<ITaxGroupDetailRepository, TaxGroupDetailRepository>();
builder.Services.AddScoped<ITCSRepository, TCSRepository>();
builder.Services.AddScoped<ITDSRepository, TDSRepository>();
builder.Services.AddScoped<ITransporterRepository, TransporterRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IHSNService, HSNService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IPaymentModeService, PaymentModeService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddScoped<IPartyAddressService, PartyAddressService>();
builder.Services.AddScoped<IPartyBankDetailsService, PartyBankDetailsService>();
builder.Services.AddScoped<IPartyContactPersonService, PartyContactPersonService>();
builder.Services.AddScoped<IPartyReportingTagService, PartyReportingTagService>();
builder.Services.AddScoped<IPartyTypeMasterService, PartyTypeMasterService>();
builder.Services.AddScoped<ICostCenterService, CostCenterService>();
builder.Services.AddScoped<IFiscalYearService, FiscalYearService>();
builder.Services.AddScoped<IGodownService, GodownService>();
builder.Services.AddScoped<IPaymentTermService, PaymentTermService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISACService, SACService>();
builder.Services.AddScoped<ISalespersonService, SalespersonService>();
builder.Services.AddScoped<ITaxGroupService, TaxGroupService>();
builder.Services.AddScoped<ITaxGroupDetailService, TaxGroupDetailService>();
builder.Services.AddScoped<ITCSService, TCSService>();
builder.Services.AddScoped<ITDSService, TDSService>();
builder.Services.AddScoped<ITransporterService, TransporterService>();
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
app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
