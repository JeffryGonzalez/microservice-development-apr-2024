using FluentValidation;

namespace CreditCardValidations;
public class CreditCardValidator
{
    public ValidationResponse ValidateCard(CardValidationRequest cardInfo, bool forRealz)
    {
        var validator = new CardValidationRequestValidator();
        var validations = validator.Validate(cardInfo);

        if (validations.IsValid)
        {
            var cardType = cardInfo.CreditCardNumber switch
            {
                var c when c.StartsWith("37") => "American Express",
                var c when c.StartsWith("3") => "Diner's Club",
                var c when c.StartsWith("4") => "Visa",
                var c when c.StartsWith("5") => "Mastercard",
                var c when c.StartsWith("6") => "Discover",
                _ => "Uknown Card Type"
            };
            return new ValidationResponse
            {
                CreditCardCompany = cardType,
                CreditCardNumber = cardInfo.CreditCardNumber,
            };
        }
        else
        {
            var exception = new BadCreditCardNumberException()
            {
                Errors = validations.ToDictionary()
            };
            throw exception;
        }
    }
}

internal class CardValidationRequestValidator : AbstractValidator<CardValidationRequest>
{
    internal CardValidationRequestValidator()
    {

        RuleFor(o => o.CreditCardNumber).NotNull().NotEmpty().CreditCard();
        RuleFor(o => o.Cvv2).NotEmpty().NotNull().Length(3).Must(c =>
        {
            return int.TryParse(c, out int result) ? result is >= 100 and < 1000 : false;
        }).WithMessage("Bad CVV2");
        RuleFor(o => o.ExpirationDate).Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Matches("^(0[1-9]|1[0-2])\\/?([0-9]{2})$")
            .Must(c =>
            {
                var parts = c.Split('/');
                var month = int.Parse(parts[0]);
                var year = int.Parse(parts[1]);
                var firstOfCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, firstOfCurrentMonth.AddMonths(1).AddSeconds(-1).Day);
                var lastDayCreditCardIsValid = new DateTime(2000 + year, month, 1).AddMonths(1).AddSeconds(-1).Date;

                return lastDayOfThisMonth <= lastDayCreditCardIsValid;
            }).WithMessage("Expired");
        RuleFor(o => o.ZipCode).NotEmpty().NotNull().Length(5); // etc.
    }
}


public record CardValidationRequest
{
    public string CreditCardNumber { get; set; } = string.Empty;
    public string Cvv2 { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

}

public record ValidationResponse
{
    public string CreditCardNumber { get; set; } = string.Empty;
    public string CreditCardCompany { get; set; } = string.Empty;

}

public class BadCreditCardNumberException : ArgumentOutOfRangeException
{
    public IDictionary<string, string[]>? Errors { get; internal set; }
}