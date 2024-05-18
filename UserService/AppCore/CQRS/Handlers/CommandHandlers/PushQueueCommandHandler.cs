using Common.Enums;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using MediatR;
using OnaxTools.Dto.Http;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models.DTOs;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
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
                    QueueName = QueueNameOrRouteKeyEnums.JobUserDetailsQueue.ToString(),
                    RoutingKey = $"{QueueNameOrRouteKeyEnums.JobUserDetailsKey.ToString()}"
                };
                _ = _messageQueueService.RabbitMqPublish<AppUserDto>(request.Entity, MqEmailOptions);


                //MqPublisherProps MqEmailOption = new()
                //{
                //    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                //    ExchangeName = ExchangeNameEnum.Upwork,
                //    QueueName = QueueNameOrRouteKeyEnums.PaymentUserDetailsQueue.ToString(),
                //    RoutingKey = $"{QueueNameOrRouteKeyEnums.PaymentUserDetailsKey.ToString()}"
                //};
                //_ = _messageQueueService.RabbitMqPublish<AppUserDto>(request.Entity, MqEmailOption);

                //MqPublisherProps MqEmailOptiond = new()
                //{
                //    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                //    ExchangeName = ExchangeNameEnum.Upwork,
                //    QueueName = QueueNameOrRouteKeyEnums.TalentUserDetailsQueue.ToString(),
                //    RoutingKey = $"{QueueNameOrRouteKeyEnums.TalentUserDetailsKey.ToString()}"
                //};
                //_ = _messageQueueService.RabbitMqPublish<AppUserDto>(request.Entity, MqEmailOptiond);

                MqPublisherProps MqEmailOptiond = new()
                {
                    ClientProvidedName = ClientProvidedNameEnum.Upwork,
                    ExchangeName = ExchangeNameEnum.Upwork,
                    QueueName = QueueNameOrRouteKeyEnums.InAppNotificationQueue.ToString(),
                    RoutingKey = $"{QueueNameOrRouteKeyEnums.InAppNotificationKey.ToString()}"
                };
                _ = _messageQueueService.RabbitMqPublish<AppUserDto>(request.Entity, MqEmailOptiond);

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
