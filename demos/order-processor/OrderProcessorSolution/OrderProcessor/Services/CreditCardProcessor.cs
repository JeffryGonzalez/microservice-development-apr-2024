namespace OrderProcessor.Services;

public class CreditCardProcessor
{
    public async Task<(Guid, DateTimeOffset)> ProcessCardAsync(string cardNumber, string cvv2, string zipCode, decimal amount)
    {
        // TODO: Actually, you know, process the credit card.
        return (Guid.NewGuid(), DateTimeOffset.UtcNow);
    }
}
