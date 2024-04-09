// See https://aka.ms/new-console-template for more information

using CreditCardValidationGrpc.Protos;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:7044");

var client = new CardValidator.CardValidatorClient(channel);

Console.WriteLine("Hit Enter to Make the Request");
Console.ReadLine();
foreach (var num in Enumerable.Range(1, 200))
{
    var request = new CardValidationRequest
    {
        CreditCarNumber = "55555555555" + num,
        ExpirationDate = "10/24",
        Cvv2 = "123",
        ZipCode = "4107"
    };

    var reply = await client.ValidateCardAsync(request);

    Console.WriteLine(reply.CreditCardNumber);
    Console.WriteLine(reply.CreditCardCompany);
}