using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OptiData.Application.Bundles.Commands.OptimizeBundles;

namespace OptiData.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptimizationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OptimizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateOptimalBundle([FromBody] OptimizeBundlesCommand command)
        {
            // send the command to the MediatR handler we built earlier
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
