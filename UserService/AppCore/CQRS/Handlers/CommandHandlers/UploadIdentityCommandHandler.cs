using Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OnaxTools.Dto.Http;
using UserService.Services;
using UserServices.AppCore.CQRS.Commands;
using UserServices.Models;
using UserServices.Persistense;
using UserServices.Services;

namespace UserServices.AppCore.CQRS.Handlers.CommandHandlers
{
    public class UploadIdentityCommandCommandHandler : IRequestHandler<UploadIdentityCommand, GenResponse<string>>
    {
        private readonly ILogger<UploadIdentityCommandCommandHandler> logger;
        private readonly IUserServicess _UserServices;
        private readonly UserDbContext _context;
        private readonly IMediator mediator;
        private readonly string _uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");


        public UploadIdentityCommandCommandHandler(ILogger<UploadIdentityCommandCommandHandler> logger, IUserServicess UserServices, UserDbContext context, IMediator mediator)
        {
            this.logger = logger;
            _UserServices = UserServices;
            _context = context;
            this.mediator = mediator;
        }

        public async Task<GenResponse<string>> Handle(UploadIdentityCommand request, CancellationToken cancellationToken)
        {
            GenResponse<string> objResp = new();
            GenResponse<string> resp = new();
            try
            {
                resp = await Upload(request.Entity.FrontView, request.Entity.UserId, request.Entity.IdType, request.Entity.HasReadIdentityPolicy);
                resp = await Upload(request.Entity.BackView, request.Entity.UserId, request.Entity.IdType, request.Entity.HasReadIdentityPolicy);
                resp = await Upload(request.Entity.Face, request.Entity.UserId, request.Entity.IdType, request.Entity.HasReadIdentityPolicy);

                if (resp.IsSuccess != null)
                {
                    objResp.IsSuccess = true;
                    objResp.Result = resp.Result;
                    objResp.Message = resp.Message;
                }
                else {
                    objResp.IsSuccess = false;
                    objResp.Result = resp.Result;
                    objResp.Message = resp.Message;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.Message = $"{AppConstants.FailedRequestError} {ex.Message}";
            }
            return objResp;

            
        }


        private async Task<GenResponse<string>> Upload(IFormFile request, string UserId, string Idtype, bool policy)
        {
            GenResponse<string> objResp = new();
            try
            {
                
                if (request == null || request.Length == 0)
                {
                    return GenResponse<string>.Failed("No file was uploaded.");
                }
                string fileName = $"{UserId}_{request.FileName}";
                if (!Directory.Exists(_uploadDirectory))
                {
                    Directory.CreateDirectory(_uploadDirectory);
                }

                string filePath = Path.Combine(_uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.CopyToAsync(stream);
                }

                // Save document details on database

                IdentityDocuments UserTrxnDocument = new()
                {   IdType = Idtype,
                    DocumentName = fileName,
                    UserId = UserId,
                    ExtensionType = Path.GetExtension(request.FileName).Split('.')[1],
                    FullDocumentNamePath = filePath,
                    HasReadIdentityPolicy = policy
                };
                await _context.IdentityDocuments.AddAsync(UserTrxnDocument);
                await _context.SaveChangesAsync();
                
                objResp.IsSuccess = true;
                objResp.Result = request.FileName;
                objResp.Message = $"File with name {request.FileName} uploaded successfully.";
             
            }
            catch (Exception ex)
            {
                this.logger.LogError(exception: ex, ex.Message);
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.Message = $" Error uploading file {request.FileName}  with messagr: {ex.Message}";
            }
            return objResp;


        }
    }
}
