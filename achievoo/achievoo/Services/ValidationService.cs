using System.Text.RegularExpressions;
using achievoo.Services.Contracts;

namespace achievoo.Services;

public class ValidationService : IValidationService
{
    private const string PhoneNumberPattern = @"^0?\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{4}$";
    private const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        var regex = new Regex(PhoneNumberPattern);
        return regex.IsMatch(phoneNumber);
    }
    
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var regex = new Regex(EmailPattern);
        return regex.IsMatch(email);
    }
}