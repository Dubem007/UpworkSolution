using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public static class AppConstants
    {
        public static string CookieUserId { get; set; } = "appuser_id";
        public static string AppSystem { get; set; } = $"Upwork-Identity{nameof(AppSystem)}";
        public static readonly string CreationSuccessResponse = "Data created successfully";
        public static readonly string CreationFailedResponse = "Failed to create data";
        public static readonly string DataRetrieveSuccessResponse = "Data successfully Retrieved";
        public static readonly string ValidateEmailSuccessResponse = "User email confirmed successfully";
        public static readonly string ValidateEmailFailedResponse = "Failed to confirm email of user";
        public static readonly string WrongEmailOrPassword = "Wrong email or password provided";
        public static readonly string LoginSuccessResponse = "Login successful";
        public static readonly string NotificationSuccessResponse = "Notification sent successful";
        public static readonly string LoginFailedResponse = "Login failed";
        public static readonly string LoginWithError = "Error in login user with messgae:";
        public static readonly string UserNotFound = "User not found";
        public static readonly string BadRequest = "Bad Request";
        public static readonly string RecordNotFound = "No record found";
        public static readonly string InvalidToken = "Invalid token";
        public static readonly string Verify2FASuccessful = "Successful 2FA verification";
        public static readonly string FailedVerify2FA = "Failed 2FA verification";
        public static readonly string InvalidExpiredToken = "The token is invalid or has expired";
        public static readonly string FailedRequest = "failed request. Please try again";
        public static readonly string FailedRequestError = "failed request due to error. Please try again";
        public static readonly string WrongPassword = "Wrong old password entered";
        public static readonly string AsDraft = "DRAFT";
        public static readonly string AsPosted = "POSTED";
        public static readonly string UserAvailableNow = "Available Now";
        public static readonly string UserNotAvailable = "Not Available";
        public static readonly string ADMIN = "Admin";
    }
}
