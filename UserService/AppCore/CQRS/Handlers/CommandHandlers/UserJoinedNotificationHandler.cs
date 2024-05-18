using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class UserJoinedNotificationHandler : IRequestHandler<UserJoinedNotificationCommand, GenResponse<bool>>
    {
        private readonly ILogger<UserJoinedNotificationHandler> logger;
        private readonly IMessageQueueService _messageQueueService;

        public UserJoinedNotificationHandler(ILogger<UserJoinedNotificationHandler> logger, IMessageQueueService messageQueueService)
        {
            this.logger = logger;
            _messageQueueService = messageQueueService;
        }
        public async Task<GenResponse<bool>> Handle(UserJoinedNotificationCommand request, CancellationToken cancellationToken)
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
                _ = _messageQueueService.RabbitMqPublish<string>($"The User {request.Entity} just joined upwork community", MqEmailOptions);

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
