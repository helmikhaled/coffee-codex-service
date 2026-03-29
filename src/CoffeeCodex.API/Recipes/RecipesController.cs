using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using CoffeeCodex.Shared.Pagination;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCodex.API.Recipes;

[ApiController]
[Route("recipes")]
public sealed class RecipesController(IGetRecipesHandler handler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<RecipeSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<RecipeSummaryDto>>> GetRecipes(
        [FromQuery] GetRecipesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.HandleAsync(request.ToQuery(), cancellationToken);

            return Ok(response);
        }
        catch (ValidationException exception)
        {
            foreach (var error in exception.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }
    }
}
