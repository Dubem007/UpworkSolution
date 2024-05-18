using IdentityServer.Models.DTOs;
using IdentityServer.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnaxTools.Dto.Http;
using OnaxTools.Http;
using System.Security.Claims;
using Common.MessageQueue.Interfaces;
using JobService.AppCore.CQRS.Commands;
using JobService.AppCore.Repository.Interface;

namespace JobService.AppCore.CQRS.Handlers.CommandHandlers
{
    public class CreatNewJobCommandHandler : IRequestHandler<CreatNewJobCommand, GenResponse<string>>
    {
        #region Variable Declarations
        private readonly ILogger<CreatNewJobCommandHandler> logger;
        private readonly IMediator _mediator;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IJobRepository _jobRepo;

        public CreatNewJobCommandHandler(ILogger<CreatNewJobCommandHandler> logger, IMediator mediator, IMessageQueueService messageQueueService, IJobRepository jobRepo)
        {
            this.logger = logger;
            _mediator = mediator;
            _messageQueueService = messageQueueService;
            _jobRepo = jobRepo;
        }

        #endregion
        public async Task<GenResponse<string>> Handle(CreatNewJobCommand request, CancellationToken cancellationToken)
        {
            GenResponse<string> objResp = new();
            try
            {
                var resp = await _jobRepo.CreateNewJob(request.Entity);

                // push the details to rabbitmq for communication of a new job creation on the services
                var pushCommandRequest = new PushQueueCommand(resp.Result);
                var responds = await _mediator.Send(pushCommandRequest);

                if (resp.IsSuccess == null)
                {
                    return GenResponse<string>.Failed("Failed to push created job communication to all users");
                }

                return resp.IsSuccess ? GenResponse<string>.Success(resp.Message) : GenResponse<string>.Failed($"{resp.Message}");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Error creating new job with message:{ex.Message}");
                return GenResponse<string>.Failed($"Error creating new job with message:{ex.Message}");
            }
        }
    }
}
