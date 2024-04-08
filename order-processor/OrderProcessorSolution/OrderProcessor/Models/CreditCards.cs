using FluentValidation;

namespace OrderProcessor.Models;


public record OrderProcessingRequest
{
    public decimal Amount { get; set; }
    public string CreditCardNumber { get; set; } = string.Empty;
    public string Cvv2 { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;



}
public class OrderProcessingRequestValidator : AbstractValidator<OrderProcessingRequest>
{
    public OrderProcessingRequestValidator()
    {
        RuleFor(o => o.Amount).GreaterThan(0);
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
                var lastDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, firstOfCurrentMonth.AddMonths(1).AddSeconds(-1).Day );
                var lastDayCreditCardIsValid = new DateTime(2000 + year, month, 1).AddMonths(1).AddSeconds(-1).Date;

                return lastDayOfThisMonth <= lastDayCreditCardIsValid;
            }).WithMessage("Expired");
        RuleFor(o => o.ZipCode).NotEmpty().NotNull().Length(5); // etc.
    }
}

public record OrderProcessingResponse
{
    public decimal Amount { get; set; }
    public string CreditCardNumber { get; set; } = string.Empty;
    public string CreditCardCompany { get; set; } = string.Empty;
    public string ApprovalCode { get; set; } = string.Empty;
    public DateTimeOffset TransactionDate { get; set; }
}