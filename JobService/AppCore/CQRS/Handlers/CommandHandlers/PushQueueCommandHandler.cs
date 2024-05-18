using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using IdentityServer.Models.DTOs;
using JobService.AppCore.CQRS.Commands;
using MediatR;
using OnaxTools.Dto.Http;

namespace JobService.AppCore.CQRS.Handlers.CommandHandlers
{
    public class PushQueueCommandHandler : IRequestHandler<PushQueueCommand, GenResponse<bool>>
    {
        private readonly ILogger<PushQueueCommandHandler> logger;
        private readonly IMessageQueueService _messageQueueService;

        public PushQueueCommandHandler(ILogger<PushQueueCommandHandler> logger, IMessageQueueService messageQueueService)
        {
            this.logger = logger;
            _messageQueueService = messageQueueService;
        }
        public async Task<GenResponse<bool>> Handle(PushQueueCommand request, CancellationToken cancellationToken)
        {
            GenResponse<bool> objResp = new();
            try
            {

                MqPublisherProps MqEmailOptions = new()
                {
                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                    ExchangeName = ExchangeNameEnum.Upwork,
                    QueueName = QueueNameOrRouteKeyEnums.InAppNotificationQueue.ToString(),
                    RoutingKey = $"{QueueNameOrRouteKeyEnums.InAppNotificationKey.ToString()}"
                };
                _ = _messageQueueService.RabbitMqPublish<string>($"The Job {request.Entity.Title} is just created", MqEmailOptions);

                objResp.IsSuccess = true;
                objResp.Result = true;
                objResp.Message = "Successfully sent";
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = false;
                objResp.Message = $"{ex.Message}";
            }
            return objResp;
            

        }
    }
}
