using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OptiData.Application.Interfaces;

namespace OptiData.Application.Assistant.Commands.AskAssistant
{
    public class AskAssistantCommandHandler : IRequestHandler<AskAssistantCommand, string>
    {
        private readonly IAssistantService _assistantService;

        public AskAssistantCommandHandler(IAssistantService assistantService)
        {
            _assistantService = assistantService;
        }

        public async Task<string> Handle(AskAssistantCommand request, CancellationToken cancellationToken)
        {
            // Takes the user's question from the command and passes it to the AI service
            var answer = await _assistantService.AskQuestionAsync(request.Question);
            
            return answer;
        }
    }
}
