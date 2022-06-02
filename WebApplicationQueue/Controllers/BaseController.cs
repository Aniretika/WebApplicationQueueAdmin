using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WebApplicationQueue.Application.Core;
using WebApplicationQueue.Entities;

namespace WebApplicationQueue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult BotOrAdminResponseOk<T>(T position, string adminActionName) where T : Position
        {
            return position.BotId is null ? RedirectToAction(adminActionName) : Ok();
        }

        protected ActionResult BotOrAdminResponseError<T>(T position, string adminActionName) where T : Position
        {
            return position.BotId is null ? RedirectToAction(adminActionName) : BadRequest();
        }

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
    }
}