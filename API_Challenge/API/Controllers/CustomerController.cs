using Application.handlers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ISender _mediator;

    public CustomerController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomerBulkPostCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);

            return result == true ? Ok(result) : BadRequest();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                errors = new[] { new { Message = "An unexpected error occurred. Please try again later." } }
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var query = new CustomerGetQuery();
            var result = await _mediator.Send(query);
            
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                errors = new[] { new { Message = "An unexpected error occurred. Please try again later." } }
            });
        }
    }

    [HttpGet("MaxId")]
    public async Task<IActionResult> GetMaxId()
    {
        try
        {
            var query = new CustomerGetMaxIdQuery();
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                errors = new[] { new { Message = "An unexpected error occurred. Please try again later." } }
            });
        }
    }
}
