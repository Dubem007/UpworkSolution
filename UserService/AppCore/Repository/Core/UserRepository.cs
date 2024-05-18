using AutoMapper;
using Common.Enums;
using Common.Extensions;
using Common.MessageQueue.Interfaces;
using Common.MessageQueue.Services;
using Common.services.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using QRCoder;
using System.Security.Cryptography;
using System.Text;
using UserServices.AppCore.Repository.Interfaces;
using UserServices.Domain.DTOs;
using UserServices.Extension.Services.BearerAuth;
using UserServices.Models;
using UserServices.Models.DTOs;
using UserServices.Persistense;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UserServices.AppCore.Repository.Core
{
    public class UserRepository: IUserRepository
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<UserRepository> _logger;
        private readonly UserDbContext _context;
        private readonly IMessageQueueService _messageQueueService;
        private readonly IAppSessionContextRepository _appSessionService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appsettings;
        private const int SecretKeyLength = 20;

        public UserRepository(ICacheService cacheService, ILogger<UserRepository> logger, UserDbContext context, IMapper mapper, IMessageQueueService messageQueueService, IAppSessionContextRepository appSessionService, IOptions<AppSettings> appsettings)
        {
            _cacheService = cacheService;
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _messageQueueService = _messageQueueService;
            _appSessionService = appSessionService;
            _appsettings = appsettings.Value;
        }
        private string AllJobsByUser(string acct, string action = "_AllJobsByUser") => $"{nameof(AllJobsByUser)}{action}:{acct}";

        public async Task<GenResponse<ContactDetailsDto>> GetContactDetails()
        {
            GenResponse<ContactDetailsDto> objResp = new();
            try
            {
                var userdetails = await _appSessionService.GetUserDetails();

                var user = new ContactDetailsDto
                {
                    UserId = ReduceString(userdetails.Result.UserId, 8),
                    Name = userdetails.Result.FullName,
                    Email = MaskEmailAddress(userdetails.Result.Email)
                };

                objResp.IsSuccess = true;
                objResp.Result = user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<ContactDetailsDto>.Failed($"An error occured getting user contact details. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<bool>> CloseMyAccount()
        {
            GenResponse<bool> objResp = new();
            try
            {
                var userdetails = await _appSessionService.GetUserDetails();

                var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == userdetails.Result.UserId);
                user.IsDeleted = true;
                await _context.SaveChangesAsync();

                objResp.IsSuccess = true;
                objResp.Result = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<bool>.Failed($"An error occured closing user account. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

   
        public async Task<GenResponse<string>> Setup2FAForUser()
        {
            GenResponse<string> objResp = new();
            var response = string.Empty;
            try
            {
                var twofauser = await Add2FAToUser();
                if (twofauser.IsSuccess)
                {
                    response = await GenerateQRCode(twofauser.Result.SecretKey, twofauser.Result.Email);
                }
               
                objResp.IsSuccess = true;
                objResp.Result = response;
            }
            catch (Exception ex)
            {
                return GenResponse<string>.Failed($"An error occured setting up User to 2FA. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        public async Task<GenResponse<bool>> Verify2FAForUser(string otp)
        {
            GenResponse<bool> objResp = new();
            var response = false;
            try
            {
                var userdetails = await _appSessionService.GetUserDetails();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userdetails.Result.UserId);
                if (user != null)
                {
                    response = await Validate2FAOTP(user.SecretKey, otp);
                }

                objResp.IsSuccess = true;
                objResp.Result = response;
            }
            catch (Exception ex)
            {
                return GenResponse<bool>.Failed($"An error occured setting up User to 2FA. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }





        private async Task<GenResponse<Add2FAResponseDto>> Add2FAToUser()
        {
            GenResponse<Add2FAResponseDto> objResp = new();
            try
            {
                var userdetails = await _appSessionService.GetUserDetails();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userdetails.Result.UserId);
                user.SecretKey = GenerateSecretKey();
                await _context.SaveChangesAsync();

                var resp = new Add2FAResponseDto()
                {
                    Email = user.Email,
                    SecretKey = user.SecretKey
                };
                objResp.IsSuccess = true;
                objResp.Result = resp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return GenResponse<Add2FAResponseDto>.Failed($"An error occured adding User to 2FA. Kindly retry", StatusCodeEnum.ServerError);
            }
            return objResp;

        }

        private async Task<bool> Validate2FAOTP(string secretKey, string otp)
        {
            long timeStep = 30; // Time step in seconds, TOTP changes every 30 seconds
            long unixTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            long counter = unixTime / timeStep;

            byte[] secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);
            byte[] counterBytes = BitConverter.GetBytes(counter);

            // Ensure big endian encoding (most significant byte first)
            if (BitConverter.IsLittleEndian)
                Array.Reverse(counterBytes);

            HMACSHA1 hmac = new HMACSHA1(secretKeyBytes);
            byte[] hash = hmac.ComputeHash(counterBytes);
            int offset = hash[hash.Length - 1] & 0x0F;
            int binary = ((hash[offset] & 0x7F) << 24) | ((hash[offset + 1] & 0xFF) << 16) | ((hash[offset + 2] & 0xFF) << 8) | (hash[offset + 3] & 0xFF);

            string otpGenerated = (binary % 1000000).ToString("D6");
            return otp == otpGenerated;
        }


        private async Task<string> GenerateQRCode(string secretKey, string email)
        {
            string issuer = "UpworkApp";
            string label = Uri.EscapeDataString($"{issuer}:{email}");

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode($"otpauth://totp/{label}?secret={secretKey}&issuer={issuer}", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            // Convert image to Base64 string
            using (MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = stream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        private static string ReduceString(string originalString, int length)
        {
            if (originalString.Length <= length)
            {
                return originalString;
            }
            else
            {
                return originalString.Substring(0, length);
            }
        }

        private static string MaskEmailAddress(string emailAddress)
        {
            int indexOfAt = emailAddress.IndexOf('@');
            if (indexOfAt > 1)
            {
                string maskedPart = new string('*', indexOfAt - 1);
                return emailAddress[0] + maskedPart + emailAddress.Substring(indexOfAt - 1);
            }
            else
            {
                return emailAddress; // Not a valid email format, return as is
            }
        }

        private static string GenerateSecretKey()
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            int keyValuelength = SecretKeyLength;
            while (0 < keyValuelength--)
            {
                sb.Append(validChars[rnd.Next(validChars.Length)]);
            }
            return sb.ToString();
        }
    }
}
