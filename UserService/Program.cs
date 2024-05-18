using Common.services.Caching;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence.ModelBuilders;
using System.Reflection;
using UserServices.Extension;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.
builder.Services.ConfigureSQLserver(builder, builder.Configuration);
builder.Services.ConfigureIdentityNew();
builder.Services.ConfigureServicesAndDI(builder);
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddHttpClient();
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.Authority = "https://localhost:5013";
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateAudience = false
//        };
//    });

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "userserviceApi.write", "userserviceApi.read");
    });
});

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
    options.DefaultScheme = "Cookies";
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = "your_google_client_id";
    options.ClientSecret = "your_google_client_secret";
})
.AddOAuth("Apple", options =>
{
    options.ClientId = "your_apple_client_id";
    options.ClientSecret = "your_apple_client_secret";
    options.AuthorizationEndpoint = "https://id.apple.com/auth/authorize";
    options.TokenEndpoint = "https://id.apple.com/auth/token";
    options.UserInformationEndpoint = "https://id.apple.com/auth/userinfo";
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Upwork UserServices APIs",
        Version = "1.0.0",
        Contact = new OpenApiContact { Email = "dondubbie007@gmail.com", Name = "UpworkUserSvc" }
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
// try { app.SeedRoleData().Wait(); } catch (Exception ex) { Console.WriteLine(ex.Message, ex); }
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upwork UserSvc  v1");
        c.DefaultModelsExpandDepth(-1);
    }); 
 }



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
