using CreditCardValidations;
using Microsoft.AspNetCore.Mvc;

using OrderProcessor.Models;
using OrderProcessor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<CreditCardProcessor>();
builder.Services.AddSingleton<CreditCardValidator>(); // TODO: Discuss
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/orders", async ([FromBody] OrderProcessingRequest request, [FromServices] CreditCardValidator validator, [FromServices] CreditCardProcessor processor) =>
{


    try
    {
        var validationReponse = validator.ValidateCard(OrderProcessingRequest.ToValidationRequest(request));

        var (approvalCode, when) = await processor.ProcessCardAsync(request.CreditCardNumber, request.Cvv2, request.ZipCode, request.Amount);

        var response = new OrderProcessingResponse
        {
            Amount = request.Amount,
            ApprovalCode = approvalCode.ToString(),
            CreditCardCompany = validationReponse.CreditCardCompany,
            CreditCardNumber = new String('*', request.CreditCardNumber.Length - 4) + request.CreditCardNumber.Substring(request.CreditCardNumber.Length - 4),
            TransactionDate = when
        };
        return Results.Ok(response);
    }
    catch (BadCreditCardNumberException ex)
    {

        return Results.ValidationProblem(ex.Errors!);
    }

});
app.Run();

public partial class Main { }