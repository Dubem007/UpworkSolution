using AutoMapper;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Common.services.Caching;
using JobService.AppCore.Repository.Core;
using JobService.AppCore.Repository.Interface;
using JobService.AppCore.Services.BearerAuth;
using JobService.AppCore.Services.Core;
using JobService.AppCore.Services.Interface;
using JobService.Domain.Configs.Mappers;
using JobService.Domain.DTOs;
using JobService.Persistense;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;

namespace JobService.Extension
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
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IAppSessionContextRepository, AppSessionContextRepositoryCommon>();
            services.AddScoped<IJobService, JobServices>();
            // services.AddScoped<ICacheService, CacheService>();
            services.AddHostedService<JobQueueService>();

           
        }

        public static void ConfigureSQLserver(this IServiceCollection services, WebApplicationBuilder builder, IConfiguration configuration)
        {
            var dbConstring = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("Default") ?? string.Empty,
                              EnvironmentVariableTarget.Process) ?? builder.Configuration.GetValue<string>("ConnectionStrings:Default");

            builder.Services.AddDbContext<JobServiceDbContext>(options => options.UseSqlServer(dbConstring, options => options.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)).UseLoggerFactory(LoggerFactory.Create(buildr =>
            {
                if (builder.Environment.IsDevelopment()) buildr.AddDebug();
            })));

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
