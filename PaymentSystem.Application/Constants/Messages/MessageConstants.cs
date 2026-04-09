
namespace PaymentSystem.Application.Constants.Messages
{
    public static class MessageConstants
    {
        public const string SuccessTitle = "Successful";
        public const string ErrorTitle = "Error!";
        public const string WarningTitle = "Warning";
        public const string InfoTitle = "Information";
        public const string NotFound = "Not Found";

        public const string AddSuccess = "The data has been successfully added.";
        public const string AddError = "An error occurred while adding the data.";
        public const string AddExists = "This data already exists.";

        public const string UpdateSuccess = "The data has been successfully updated.";
        public const string UpdateError = "An error occurred while updating the data.";
        public const string UpdateNotFound = "No data found to update.";

        public const string DeleteSuccess = "The data has been successfully deleted.";
        public const string DeleteError = "An error occurred while deleting the data.";
        public const string DeleteExists = "This data already exists.";

        public const string IsDeletedSuccess = "The data has been successfully set Deleted.";
        public const string IsDeletedError = "An error occurred while setting Deleted to the data.";
        public const string IsDeleteNotFound = "This data was not found.";

        public const string NotDeletedSuccess = "The data has been successfully set Not Deleted.";
        public const string NotDeleteError = "An error occurred while setting Not Deleted to the data.";
        public const string NotDeletedNotFound = "This data was not found.";

        public const string IsActiveSuccess = "The data has been successfully set Active.";
        public const string IsActiveError = "An error occurred while setting Active to the data.";
        public const string IsActiveNotFound = "This data was not found.";

        public const string IsInActiveSuccess = "The data has been successfully set InActive.";
        public const string IsInActiveError = "An error occurred while setting InActive to the data.";
        public const string IsInActiveNotFound = "This data was not found.";

        public const string LoginError = "Your login attempt failed. Please try again.";
        public const string RegisterError = "An error occurred during registration. Please fill in all required fields and try again. If you encounter another error during registration, please feel free to contact us via our email addresses. We apologize for the inconvenience.";
        public const string LoginConfirmMailError = "There is an error in the 6-digit verification code you entered for email verification. Please check the verification code and try again.";
        public const string ConfirmMailError = "There is an error in the 6-digit confirmation code you entered for email verification. Please check the confirmation code and try again.";
        public const string ResetPasswordSendCodeError = "Please enter your valid email address.";
        public const string ResetPasswordSendCodeSuccess = "Please check your email address. A password reset code has been sent to your email address. If you cannot find the code, check your spam/junk folder.";
        public const string ResetPasswordError = "An error occurred while resetting your password. Please ensure you have entered the required information correctly and try again. If the error persists, please notify us via the email address on our contact page. We apologize for the inconvenience.";
        public const string ChangePasswordError = "An error occurred while changing your password. Please try again. If the problem persists, please feel free to report it using the email address on our contact page. We apologize for the inconvenience.";
        public const string ChangePasswordSuccess = "Your password has been successfully changed.";
        public const string UpdateProfileError = "An error occurred while updating your profile information. Please fill in all fields again and try again. If the problem persists, please feel free to report it using the email address on our contact page. We apologize for the inconvenience.";
        public const string UpdateProfileSuccess = "Your profile information has been successfully updated.";
    }
}
