using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Microsoft.Extensions.Options;
using NotificationService.Models.DTO;
using NotificationService.Repository;
using OnaxTools.Dto.Http;
using System.Text.Json;

namespace NotificationService.AppCore.Services.Core
{
    public class EmailQueueService : BackgroundService
    {
    
        private readonly ILogger<EmailQueueService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _appsettings;

        public EmailQueueService(ILogger<EmailQueueService> logger, IServiceProvider serviceProvider, IOptions<AppSettings> appsettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appsettings = appsettings.Value;
        }

        public async Task<GenResponse<bool>> RabbitMQEmailService(CancellationToken ct = default)
        {
            GenResponse<bool> objResp = new();

           
            try
            {
                FetchQueueProps MqSubAcctCreationOptions = new()
                {
                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                    ExchangeName = ExchangeNameEnum.Upwork,
                    QueueName = QueueNameOrRouteKeyEnums.EmailMessages.ToString(),
                    RoutingKey = QueueNameOrRouteKeyEnums.GeneralEmailKey.ToString(),
                    IsAutoAcknowledged = true
                };

                var scope = _serviceProvider.CreateScope();
                var msgQueueSvc = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();
                var usrReposSvc = scope.ServiceProvider.GetRequiredService<IMessageRepository>();

               
                GenResponse<string> subAcctsCreation = await msgQueueSvc.FetchAndProcessMsgFromQueue(MqSubAcctCreationOptions, ct: ct);
                if (subAcctsCreation != null && subAcctsCreation.IsSuccess)
                {
                    EmaiReqsDto emailReq = JsonSerializer.Deserialize<EmaiReqsDto>(subAcctsCreation.Result);
                    if (emailReq != null)
                    {
                        var objResult = await usrReposSvc.SendEmailAsync(emailReq);
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
                                _ = await msgQueueSvc.RabbitMqPublish<EmaiReqsDto>(emailReq, jobuserCreatePubOptions);
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

                await this.RabbitMQEmailService(stoppingToken);

                Thread.Sleep(1000);
                stopwatch.Stop();
                Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
