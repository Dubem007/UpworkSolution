using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using JobService.AppCore.Repository.Interface;
using JobService.Domain.Dtos;
using JobService.Domain.DTOs;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using System.Text.Json;

namespace JobService.AppCore.Services.Core
{
    public class JobQueueService : BackgroundService
    {
    
        private readonly ILogger<JobQueueService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _appsettings;

        public JobQueueService(ILogger<JobQueueService> logger, IServiceProvider serviceProvider, IOptions<AppSettings> appsettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appsettings = appsettings.Value;
        }

        public async Task<GenResponse<bool>> RabbitMQCreateJobUser(CancellationToken ct = default)
        {
            GenResponse<bool> objResp = new();

            FetchQueueProps MqSubAcctCreationOptions = new()
            {
                ClientProvidedName = ClientProvidedNameEnum.Upwork,
                ExchangeName = ExchangeNameEnum.Upwork,
                QueueName = QueueNameOrRouteKeyEnums.JobUserDetailsQueue.ToString(),
                RoutingKey = QueueNameOrRouteKeyEnums.JobUserDetailsKey.ToString(),
                IsAutoAcknowledged = _appsettings.MsgQueue.IsAutoAcknowledged
            };
            try
            {
                var scope = _serviceProvider.CreateScope();
                var msgQueueSvc = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();
                var jobReposSvc = scope.ServiceProvider.GetRequiredService<IJobRepository>();


                GenResponse<string> subAcctsCreation = await msgQueueSvc.FetchAndProcessMsgFromQueue(MqSubAcctCreationOptions, ct: ct);
                if (subAcctsCreation != null && subAcctsCreation.IsSuccess)
                {
                    AppUserDto subAcctModel = JsonSerializer.Deserialize<AppUserDto>(subAcctsCreation.Result);
                    if (subAcctModel != null)
                    {
                        var objResult = await jobReposSvc.CreateUsers(subAcctModel);
                        objResp.IsSuccess = objResult.IsSuccess; objResp.Error = objResult.Error; objResp.Message = objResult.Message;
                        if (objResult.IsSuccess == false)
                        {
                            if (objResult != null && !string.IsNullOrWhiteSpace(objResult.Error))
                            {
                                MqPublisherProps jobuserCreatePubOptions = new()
                                {
                                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                                    ExchangeName = ExchangeNameEnum.Upwork,
                                    QueueName = QueueNameOrRouteKeyEnums.JobUserDetailsQueue.ToString(),
                                    RoutingKey = QueueNameOrRouteKeyEnums.JobUserDetailsKey.ToString(),
                                    IsDurable = true
                                };
                                _ = await msgQueueSvc.RabbitMqPublish<AppUserDto>(subAcctModel, jobuserCreatePubOptions);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return GenResponse<bool>.Failed(ex.Message);
            }
            return objResp;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                System.Diagnostics.Stopwatch stopwatch = new();
                stopwatch.Start();

                await this.RabbitMQCreateJobUser(stoppingToken);

                Thread.Sleep(_appsettings.MsgQueue.DelayInMilliseconds);
                stopwatch.Stop();
                Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
