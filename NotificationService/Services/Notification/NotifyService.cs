using Common.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using NotificationService.Services.Interface;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;

namespace NotificationService.Services.Notification
{
    public class NotifyService: INotifyService
    {
        private readonly ILogger<NotifyService> _logger;

        public NotifyService(ILogger<NotifyService> logger)
        {
            _logger = logger;
        }

        public async Task<GenResponse<bool>> InAppCommAsync(string payload)
        {
            GenResponse<bool> objResp = new();
            if (string.IsNullOrEmpty(payload))
            {
                _logger.LogInformation("Parameter cannot be null");
                return GenResponse<bool>.Failed($"Parameter cannot be null", StatusCodeEnum.ServerError);
            }

            try
            {
                var connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7123/notificationhub") // URL of your NotificationHub
                    .Build();

                connection.On<string>("ReceiveNotification", (message) =>
                {
                    _logger.LogInformation($"Received notification: {message}");
                });

                try
                {
                    await connection.StartAsync();
                    _logger.LogInformation("Connection established. Waiting for notifications...");
                    // Send notification
                    await connection.InvokeAsync("SendNotification", payload);

                    _logger.LogInformation("Notification sent.");
                    objResp.IsSuccess = true;
                    objResp.Result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                    return GenResponse<bool>.Failed($"An error occured for in app communication. Kindly retry", StatusCodeEnum.ServerError);
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send notification: {ex.Message}", ex);
                return GenResponse<bool>.Failed($"An error occured for in app communication. Kindly retry", StatusCodeEnum.ServerError);
            }

            return objResp;
        }

       
    }
}
