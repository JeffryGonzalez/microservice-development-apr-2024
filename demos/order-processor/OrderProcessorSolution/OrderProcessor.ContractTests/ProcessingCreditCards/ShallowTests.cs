

using Alba;
using OrderProcessor.Models;

namespace OrderProcessor.ContractTests.ProcessingCreditCards;
public class ShallowTests
{
    [Fact]
    public async Task HappyPath()
    {
        var host = await AlbaHost.For<Program>();

        var request = new OrderProcessingRequest
        {
            Amount = 123.45M,
            CreditCardNumber = "4012888888881881",
            ExpirationDate = "04/28", // ??
            Cvv2 = "123",
            ZipCode = "44107"
        };

        var expected = new OrderProcessingResponse
        {
            Amount = request.Amount,
            ApprovalCode = "X",
            CreditCardCompany = "Visa",
            CreditCardNumber = "************1881",
            // TransactionDate
        };

        var response = await host.Scenario(api =>
        {
            api.Post.Json(request).ToUrl("/orders");
            api.StatusCodeShouldBeOk();
        });

        var actualResponse = response.ReadAsJson<OrderProcessingResponse>();
        Assert.NotNull(actualResponse);

        Assert.Equal(expected.Amount, actualResponse.Amount);
        Assert.Equal(expected.CreditCardNumber, actualResponse.CreditCardNumber);
        Assert.Equal(expected.CreditCardCompany, actualResponse.CreditCardCompany);
        // Warning: Flaky
        Assert.Equal(DateTimeOffset.Now.UtcDateTime.ToString(), actualResponse.TransactionDate.UtcDateTime.ToString());

        var txId = Guid.Parse(actualResponse.ApprovalCode); // WTH? At least this will throw if it isn't a GUID?

    }
}
