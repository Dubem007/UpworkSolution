using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Microsoft.Extensions.Options;
using NotificationService.Models.DTO;
using NotificationService.Repository;
using NotificationService.Services.Interface;
using OnaxTools.Dto.Http;
using System.Text.Json;

namespace NotificationService.AppCore.Services.Core
{
    public class InAppQueueService : BackgroundService
    {
    
        private readonly ILogger<InAppQueueService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _appsettings;

        public InAppQueueService(ILogger<InAppQueueService> logger, IServiceProvider serviceProvider, IOptions<AppSettings> appsettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appsettings = appsettings.Value;
        }

        public async Task<GenResponse<bool>> RabbitMQInAppNotify(CancellationToken ct = default)
        {
            GenResponse<bool> objResp = new();

           
            try
            {
                FetchQueueProps MqSubAcctCreationOptions = new()
                {
                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                    ExchangeName = ExchangeNameEnum.Upwork,
                    QueueName = QueueNameOrRouteKeyEnums.InAppNotificationQueue.ToString(),
                    RoutingKey = QueueNameOrRouteKeyEnums.InAppNotificationKey.ToString(),
                    IsAutoAcknowledged = true
                };

                var scope = _serviceProvider.CreateScope();
                var msgQueueSvc = scope.ServiceProvider.GetRequiredService<IMessageQueueService>();
                var usrReposSvc = scope.ServiceProvider.GetRequiredService<INotifyService>();

               
                GenResponse<string> subAcctsCreation = await msgQueueSvc.FetchAndProcessMsgFromQueue(MqSubAcctCreationOptions, ct: ct);
                if (subAcctsCreation != null && subAcctsCreation.IsSuccess)
                {
                    var emailReq = JsonSerializer.Deserialize<string>(subAcctsCreation.Result);
                    if (emailReq != null)
                    {
                        var objResult = await usrReposSvc.InAppCommAsync(emailReq);
                        objResp.IsSuccess = objResult.IsSuccess; objResp.Error = objResult.Error; objResp.Message = objResult.Message;
                        if (objResult.IsSuccess == false)
                        {
                            if (objResult != null && !string.IsNullOrWhiteSpace(objResult.Error))
                            {
                                MqPublisherProps jobuserCreatePubOptions = new()
                                {
                                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                                    ExchangeName = ExchangeNameEnum.Upwork,
                                    QueueName = QueueNameOrRouteKeyEnums.InAppNotificationQueue.ToString(),
                                    RoutingKey = QueueNameOrRouteKeyEnums.InAppNotificationKey.ToString(),
                                    IsDurable = true
                                };
                                _ = await msgQueueSvc.RabbitMqPublish<string>(emailReq, jobuserCreatePubOptions);
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

                await this.RabbitMQInAppNotify(stoppingToken);

                Thread.Sleep(1000);
                stopwatch.Stop();
                Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
