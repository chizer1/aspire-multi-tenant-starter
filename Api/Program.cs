using System.Security.Claims;
using Api;
using Api.Features.Admin.Tenants.CreateTenant;
using Api.Features.Admin.Tenants.GetTenants;
using Api.Features.Client.Products.AddProduct;
using Api.Features.Client.Products.DeleteProduct;
using Api.Features.Client.Products.GetProducts;
using Api.Features.Client.Products.UpdateProduct;
using Db;
using Finbuckle.MultiTenant;
using FluentValidation;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.Services.AddControllers();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var configBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

var configuration = builder.Configuration;

builder.AddServiceDefaults();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetProductsHandler).Assembly)
);
builder.Services.AddValidatorsFromAssemblyContaining<GetProductsValidator>();

services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseConnectionString =
        config["ConnectionStrings:Base"]
        ?? throw new InvalidOperationException("Base connection string is not configured.");

    return new DbLocatorWrapper(baseConnectionString);
});

services.AddHostedService<DbLocatorInitializer>();
builder.Services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformer>();

builder
    .Services.AddMultiTenant<TenantInfo>()
    .WithClaimStrategy("tenant_code")
    .WithStore(
        ServiceLifetime.Singleton,
        sp => new TenantStore(sp.GetRequiredService<DbLocatorWrapper>())
    )
    .WithHostStrategy();

builder.Services.AddSingleton(sp => new TenantStore(sp.GetRequiredService<DbLocatorWrapper>()));

builder.Services.AddEndpointsApiExplorer();
services.AddApplicationOpenApi(configuration);

services.AddKeycloakWebApiAuthentication(
    configuration,
    options =>
    {
        options.Audience = "workspaces-client";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = ClaimTypes.Role
        };
    }
);

services.AddAuthorizationBuilder().AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<ITenantDbConnectionProvider, TenantDbConnectionProvider>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContextFactory<ClientDbContext>();
builder.Services.AddScoped<IClientDbContextFactory, ClientDbContextFactory>();

builder.Services.AddScoped(async sp =>
{
    var factory = sp.GetRequiredService<IDbContextFactory<ClientDbContext>>();
    var tenantProvider = sp.GetRequiredService<ITenantDbConnectionProvider>();
    var clientDb = await tenantProvider.ClientDb();

    var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
    optionsBuilder.UseSqlServer(clientDb.ConnectionString);

    return new ClientDbContext(optionsBuilder.Options);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseApplicationOpenApi();

app.UseRouting();
app.UseAuthentication();
app.UseMultiTenant();
app.UseAuthorization();

app.UseCors("AllowAll");

// endpoints
app.MapGetProducts();
app.MapAddProduct();
app.MapUpdateProduct();
app.MapDeleteProduct();
app.MapGetTenants();
app.MapCreateTenant();

app.Run();
