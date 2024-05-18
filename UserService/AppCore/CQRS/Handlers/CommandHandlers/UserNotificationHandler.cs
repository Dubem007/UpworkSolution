using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class UserNotificationHandler : IRequestHandler<UserNotificationCommand, GenResponse<bool>>
    {
        private readonly ILogger<UserNotificationHandler> logger;
        private readonly IMessageQueueService _messageQueueService;

        public UserNotificationHandler(ILogger<UserNotificationHandler> logger, IMessageQueueService messageQueueService)
        {
            this.logger = logger;
            _messageQueueService = messageQueueService;
        }
        public async Task<GenResponse<bool>> Handle(UserNotificationCommand request, CancellationToken cancellationToken)
        {
            GenResponse<bool> objResp = new();
            try
            {
                MqPublisherProps MqEmailOptions = new()
                {
                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                    ExchangeName = ExchangeNameEnum.Upwork,
                    QueueName = QueueNameOrRouteKeyEnums.EmailMessages.ToString(),
                    RoutingKey = $"{QueueNameOrRouteKeyEnums.GeneralEmailKey.ToString()}"
                };
                _ = _messageQueueService.RabbitMqPublish<EmaiReqsDto>(request.Entity, MqEmailOptions);

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
