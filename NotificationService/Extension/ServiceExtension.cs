using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using NotificationService.AppCore.Services.Core;
using NotificationService.Models.DTO;
using NotificationService.Repository;
using NotificationService.Services.EmailServices;
using NotificationService.Services.Interface;
using NotificationService.Services.Notification;
using System;

namespace NotificationService.Extension
{
    public static class ServiceExtension
    {
        public static void ConfigureServicesAndDI(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
            services.Configure<EmailConfig>(builder.Configuration.GetSection(nameof(EmailConfig)));

            services.AddScoped<IMessageQueueService, MessageQueueService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddHttpContextAccessor();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotifyService, NotifyService>();
            services.AddHostedService<EmailQueueService>();
            services.AddHostedService<InAppQueueService>();

        }

        public static void ConfigureSQLserver(this IServiceCollection services, WebApplicationBuilder builder, IConfiguration configuration)
        {
            var dbConstring = Environment.GetEnvironmentVariable(builder.Configuration.GetConnectionString("Default") ?? string.Empty,
                              EnvironmentVariableTarget.Process) ?? builder.Configuration.GetValue<string>("ConnectionStrings:Default");

        }
    }
}
