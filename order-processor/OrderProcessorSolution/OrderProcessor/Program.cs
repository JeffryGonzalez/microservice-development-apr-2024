using Microsoft.AspNetCore.Mvc;

using OrderProcessor.Models;
using OrderProcessor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<CreditCardProcessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/orders", async ([FromBody] OrderProcessingRequest request, [FromServices] CreditCardProcessor processor) =>
{
    var validator = new OrderProcessingRequestValidator();
    var results = validator.Validate(request);
    if(!results.IsValid)
    {
        
        return Results.ValidationProblem(results.ToDictionary());
    }
    var cardType = request.CreditCardNumber switch
    {
        var c when c.StartsWith("37") => "American Express",
        var c when c.StartsWith("3") => "Diner's Club",
        var c when c.StartsWith("4") => "Visa",
        var c when c.StartsWith("5") => "Mastercard",
        var c  when c.StartsWith("6") => "Discover",
        _ => "Uknown Card Type"
    };


    var (approvalCode, when) = await processor.ProcessCardAsync(request.CreditCardNumber, request.Cvv2, request.ZipCode, request.Amount);

    var response = new OrderProcessingResponse
    {
        Amount = request.Amount,
        ApprovalCode = approvalCode.ToString(),
        CreditCardCompany = cardType,
        CreditCardNumber = new String('*', request.CreditCardNumber.Length - 4) + request.CreditCardNumber.Substring(request.CreditCardNumber.Length - 4),
        TransactionDate = when
    };
    return Results.Ok(response);
});
app.Run();

public partial class Main { }