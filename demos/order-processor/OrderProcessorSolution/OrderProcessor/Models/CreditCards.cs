using CreditCardValidations;

namespace OrderProcessor.Models;


public record OrderProcessingRequest
{
    public decimal Amount { get; set; }
    public string CreditCardNumber { get; set; } = string.Empty;
    public string Cvv2 { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;


    public static CardValidationRequest ToValidationRequest(OrderProcessingRequest request)
    {
        return new CardValidationRequest
        {
            CreditCardNumber = request.CreditCardNumber,
            Cvv2 = request.Cvv2,
            ExpirationDate = request.ExpirationDate,
            ZipCode = request.ZipCode,
        };
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