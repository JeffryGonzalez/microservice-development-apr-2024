using CreditCardValidationGrpc.Protos;
using Grpc.Core;

namespace CreditCardValidationGrpc.Services;

public class CardValidatorService : CardValidator.CardValidatorBase
{
    public override async Task<CardValidationResponse> ValidateCard(CardValidationRequest request, ServerCallContext context)
    {
        var response = new CardValidationResponse
        {
            CreditCardCompany = "Visa",
            CreditCardNumber = request.CreditCarNumber
        };
        return response;
    }
}
