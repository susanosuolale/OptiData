using MediatR;

namespace OptiData.Application.Assistant.Commands.AskAssistant
{
    public class AskAssistantCommand : IRequest<string>
    {
        public string Question { get; set; }
    }
}
