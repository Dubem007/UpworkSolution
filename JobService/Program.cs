using JobService.AppCore.Services.Core;
using JobService.Extension;
using Common.services.Caching;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;
using Microsoft.OpenApi.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using System.Reflection;
using JobService.Persistence.ModelBuilders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

#region Serilog configuration

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json").Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console(new JsonFormatter(), LogEventLevel.Information)
    .CreateLogger();

#endregion
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureSQLserver(builder, builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureServicesAndDI(builder);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5013";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "userserviceApi.write", "userserviceApi.read");
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Upwork JobService APIs",
        Version = "1.0.0",
        Contact = new OpenApiContact { Email = "applicationdev@upwork.com", Name = "UpworkJobSvc" }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
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
                },

            },
            new List<string>()
        }
    });
});

builder.AddCachingService();

builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader().WithMethods("*");
    });
});
var app = builder.Build();
try { app.SeedRoleData().Wait(); } catch (Exception ex) { Console.WriteLine(ex.Message, ex); }
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upwork JobSvc  v1");
        c.DefaultModelsExpandDepth(-1);
    });


app.Use((context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)context.Request.Headers["Origin"] });
        context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type,contentType, Accept, Authorization, XApiKey,session,Session" });
        context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
        context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
    }
    return next.Invoke();
});
app.UseCors(builder => builder.WithOrigins("*"));
app.UseCors("DefaultCorsPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
