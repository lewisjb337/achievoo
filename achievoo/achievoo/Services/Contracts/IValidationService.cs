namespace achievoo.Services.Contracts;

public interface IValidationService
{
    bool IsValidPhoneNumber(string phoneNumber);
    bool IsValidEmail(string email);
}