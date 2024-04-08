using CreditCardValidations;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardValidationApi.Controllers;

public class CardValidationsController : ControllerBase
{
    [HttpPost("/card-validator")] // # Yuck!
    public IActionResult ValidateCard([FromBody] CardValidationRequest request)
    {
        try
        {

            var validator = new CreditCardValidator();
            var results = validator.ValidateCard(request);


            return Ok(results);
        }
        catch (BadCreditCardNumberException ex)
        {

            return BadRequest(ex.Errors);
        }
    }
}
