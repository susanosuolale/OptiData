using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OptiData.Application.Assistant.Commands.AskAssistant;
using OptiData.Presentation.Models;

namespace OptiData.Presentation.Controllers
{
    public class AssistantController : Controller
    {
        private readonly IMediator _mediator;

        public AssistantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // If a user manually types /Assistant into the URL bar, 
        // we safely redirect them back to the Home page where the widget lives.
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // Handles the form submission from the Home page widget
        [HttpPost]
        public async Task<IActionResult> Index(AskAssistantCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Question))
            {
                // TempData: shared dictionary that all parts of the app have access to
                // like the controllers, views, and pages
                TempData["AssistantError"] = "Please enter a valid question.";
                return RedirectToAction("Index", "Home");
            }
            
            // We pass the bound envelope directly to MediatR to handle
            var answer = await _mediator.Send(command);

            TempData["AssistantAnswer"] = answer;
            TempData["AssistantQuestion"] = command.Question;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AskAjax([FromBody] AskAssistantAjaxCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Question))
            {
                return Json(new { success = false, error = "Please enter a valid question." });
            }
            
            var mediatrCommand = new AskAssistantCommand { Question = command.Question };
            var answer = await _mediator.Send(mediatrCommand);

            return Json(new { success = true, answer = answer });
        }
    }

    public class AskAssistantAjaxCommand
    {
        public string Question { get; set; }
    }
}
