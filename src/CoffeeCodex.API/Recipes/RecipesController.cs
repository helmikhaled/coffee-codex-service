using CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;
using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;
using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using CoffeeCodex.Shared.Pagination;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCodex.API.Recipes;

[ApiController]
[Route("recipes")]
public sealed class RecipesController(
    IRecordRecipeViewHandler recordRecipeViewHandler,
    IGetRecipesHandler getRecipesHandler,
    IGetRandomRecipeHandler getRandomRecipeHandler,
    IGetRecipeDetailHandler getRecipeDetailHandler) : ControllerBase
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
            var response = await getRecipesHandler.HandleAsync(request.ToQuery(), cancellationToken);

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

    [HttpGet("random")]
    [ProducesResponseType(typeof(RandomRecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RandomRecipeDto>> GetRandomRecipe(
        CancellationToken cancellationToken)
    {
        var response = await getRandomRecipeHandler.HandleAsync(new GetRandomRecipeQuery(), cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPost("{id:guid}/view")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RecordRecipeView(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var wasRecorded = await recordRecipeViewHandler.HandleAsync(
                new RecordRecipeViewCommand(id),
                cancellationToken);

            if (!wasRecorded)
            {
                return NotFound();
            }

            return NoContent();
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecipeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailDto>> GetRecipeById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await getRecipeDetailHandler.HandleAsync(
                new GetRecipeDetailQuery(id),
                cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

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
