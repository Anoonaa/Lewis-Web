using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;
using LewisStores.Api.Data;
using LewisStores.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext with SQLite
var defaultConnection = "Data Source=lewis.db";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRINGS__DEFAULT_CONNECTION")
    ?? defaultConnection;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Configure Mock JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "LewisStoresMockIssuer",
            ValidAudience = "LewisStoresMockAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsAMockSuperSecretKeyForLewisStoresApisThatIsAtLeast32Bytes"))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lewis Stores API",
        Version = "v1",
        Description = "Comprehensive API documentation for Lewis Stores ecommerce services.\n\nIncludes catalog, cart, orders, credit applications, and authentication endpoints.",
        Contact = new OpenApiContact
        {
            Name = "Lewis Stores Engineering",
            Email = "api-support@lewisstores.local"
        },
        License = new OpenApiLicense
        {
            Name = "Internal Use"
        }
    });

    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    c.SupportNonNullableReferenceTypes();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT bearer authentication.\n\nEnter only the token value below; Swagger UI will prepend 'Bearer '.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.OperationFilter<AuthorizeCheckOperationFilter>();

    c.TagActionsBy(api =>
    {
        if (api.GroupName is not null)
        {
            return new[] { api.GroupName };
        }

        if (api.ActionDescriptor.RouteValues.TryGetValue("controller", out var controllerName) && !string.IsNullOrWhiteSpace(controllerName))
        {
            return new[] { controllerName };
        }

        return new[] { "Endpoints" };
    });

    c.OrderActionsBy(api => $"{api.GroupName}_{api.HttpMethod}_{api.RelativePath}");
});

var app = builder.Build();

var resetDatabaseOnStart = builder.Configuration.GetValue<bool>("ResetDatabaseOnStart")
    || string.Equals(Environment.GetEnvironmentVariable("RESET_DATABASE_ON_START"), "true", StringComparison.OrdinalIgnoreCase);

var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(renderPort))
{
    app.Urls.Add($"http://0.0.0.0:{renderPort}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true) // Always show swagger for this mock app
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "docs/{documentName}/openapi.json";
    });

    app.UseStaticFiles();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/docs/v1/openapi.json", "Lewis Stores API v1");
        c.RoutePrefix = "docs";
        c.DocumentTitle = "Lewis Stores API Documentation";
        c.DocExpansion(DocExpansion.List);
        c.DefaultModelsExpandDepth(-1);
        c.EnableDeepLinking();
        c.EnableFilter();
        c.DisplayRequestDuration();
        c.EnablePersistAuthorization();
        c.InjectStylesheet("/swagger-ui/custom.css");
        c.InjectJavascript("/swagger-ui/custom.js", "text/javascript");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create and seed database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (resetDatabaseOnStart)
    {
        db.Database.EnsureDeleted();
    }

    db.Database.EnsureCreated();
}

app.Run();
