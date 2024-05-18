using AutoMapper;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Common.services.Caching;
using UserServices.AppCore.Services.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using UserServices.AppCore.Repository.Core;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Domain.Configs.Mappers;
using UserServices.Domain.DTOs;
using UserServices.Extension.Services.BearerAuth;
using UserServices.Models;
using UserServices.Persistense;
using UserServices.Services;
using Microsoft.Extensions.Hosting;
using Amazon.S3;
using Azure.Storage.Blobs;
using UserServices.AppCore.Services;
using UserServices.Services.AWS;
using UserService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UserServices.Extension
{
    public static class ServiceExtension
    {
        public static void ConfigureServicesAndDI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));


            var configMap = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });
            var mapper = configMap.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IMessageQueueService, MessageQueueService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAppSessionContextRepository, AppSessionContextRepositoryCommon>();
            services.AddScoped<IUserServicess, UserServicess>();
            // services.AddHostedService<UserQueueService>();
            services.AddHostedService<ProfileUpdateServices>();
            services.AddTransient<IAwsS3Client, AwsS3Client>();


        }

        public static void ConfigureIdentityNew(this IServiceCollection services)
        {
            var builder = services.AddIdentity<ApplicationUser, UserRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureSQLserver(this IServiceCollection services, WebApplicationBuilder builder, IConfiguration configuration)
        {
            var dbConstring = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("Default") ?? string.Empty,
                              EnvironmentVariableTarget.Process) ?? builder.Configuration.GetValue<string>("ConnectionStrings:Default");

            builder.Services.AddDbContext<UserDbContext>(options => options.UseSqlServer(dbConstring, options => options.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)).UseLoggerFactory(LoggerFactory.Create(buildr =>
            {
                if (builder.Environment.IsDevelopment()) buildr.AddDebug();
            })));

        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("AppSettings");
            var jwtUserSecret = jwtSettings.GetSection("SecretKey").Value ?? string.Empty;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("ValidIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("ValidAudience").Value,
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSecret))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => { return Task.CompletedTask; }
                };
            });
        }
        public static void ConfigureOpenId(this IServiceCollection services)
        {
            services.AddHttpClient("MovieAPIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5010/"); // API GATEWAY URL
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });


            // 2 create an HttpClient used for accessing the IDP
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5005/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });
        }
    }
}
