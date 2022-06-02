using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueueWebApplicationTest.Application.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationQueue.Application.Commands.Delete;
using WebApplicationQueue.Application.Commands.Update;
using WebApplicationQueue.Application.Queries.GetItem;
using WebApplicationQueue.Application.Queries.GetList;
using WebApplicationQueue.Entities;
using WebApplicationQueue.Controllers;

namespace WebApplicationQueue.Controllers
{

    public class QueueController : BaseController
    {
        [HttpPost("CreatePosition")]
        public async Task<IActionResult> CreatePositionFromForm([FromForm] Position position) =>
           await CreatePosition(position);

        [HttpPost("CreatePositionPost")]
        public async Task<IActionResult> CreatePositionFromBody([FromBody] Position position) =>
            await CreatePosition(position);

        [HttpPost]
        private async Task<IActionResult> CreatePosition(Position position)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Position was not created";
                return BotOrAdminResponseError(position, nameof(GetQueue));
            }

            await Mediator.Send(new CreatePosition.CommandCreate { Position = position });
            TempData["success"] = "Position created successfully";
            return BotOrAdminResponseOk(position, nameof(GetQueue));
        }

        [HttpGet]
        [Route("CreatePosition")]
        public IActionResult CreatePosition()
        {
            return View();
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> EditPosition(Guid id)
        {
            if (id == default)
            {
                TempData["error"] = "Position is not searched successfully";
                return RedirectToAction(nameof(GetQueue));
            }
            //redirecrt to main list + temp data
            var positionResponse = await Mediator.Send(new GetItem.Query { Id = id });
            if (positionResponse is null)
            {
                TempData["error"] = "Position is not updated";
                return RedirectToAction(nameof(GetQueue));
            }
            return View(positionResponse);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditPosition([FromForm] Position position, Guid id)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Position was not updated";
                return BotOrAdminResponseError(position, nameof(GetQueue));
            }

            position.Id = id;
            await Mediator.Send(new UpdatePosition.CommandUpdate { Position = position });
            TempData["success"] = "Position updated successfully";
            return BotOrAdminResponseOk(position, nameof(GetQueue));
        }

        [HttpGet]
        [Route("DeletePosition/{id}")]
        public async Task<IActionResult> DeletePosition(Guid id)
        {
            var positionResponse = await Mediator.Send(new GetItem.Query { Id = id });
            if (id == Guid.Empty && positionResponse == null)
            {
                TempData["error"] = "Position is not searched";
                return RedirectToAction(nameof(GetQueue));
            }

            return View(positionResponse);
        }

        [HttpPost]
        [Route("DeletePosition/{id}")]
        public async Task<IActionResult> DeletePositionPOST(Guid id)
        {
            var positionResponse = await Mediator.Send(new GetItem.Query { Id = id });

            if (id == Guid.Empty || positionResponse == null)
            {
                TempData["error"] = "Position was not searched";
                return BotOrAdminResponseError(positionResponse, nameof(GetQueue));
            }

            TempData["success"] = "Position deleted successfully";
            await Mediator.Send(new DeletePosition.CommandDelete { Id = id });
            return BotOrAdminResponseOk(positionResponse, nameof(GetQueue));
        }

        [HttpGet("GetByAuthorId/{authorId}")]
        public async Task<JsonResult> GetPositionByAuthorId(string authorId)
        {
            List<int> result = new();
            List<Position> mainQueueList = await Mediator.Send(new MainQueue.Query());

            List<Position> positionsByAuthor = mainQueueList.FindAll(pos => pos.AuthorId != null && pos.AuthorId.Equals(authorId));

            foreach (var position in positionsByAuthor)
            {
                result.Add(position.NumberInTheQueue);
            }
            return new JsonResult(result);
        }

        [HttpGet("GetLastNumberFromQueue")]
        public async Task<JsonResult> GetLastNuberInQueue()
        {
            var mainQueueList = await Mediator.Send(new MainQueue.Query());
            var result = mainQueueList.Max(pos => pos.NumberInTheQueue);
            return new JsonResult(result);
        }

        [HttpGet("GetQueue")]
        public async Task<IActionResult> GetQueue()
        {
            bool isBotRequest = Request.Headers.ContainsKey("IsBot");
            var mainQueueList = await Mediator.Send(new MainQueue.Query());
            return isBotRequest ? new JsonResult(mainQueueList) : View(mainQueueList);
        }
    }
}
