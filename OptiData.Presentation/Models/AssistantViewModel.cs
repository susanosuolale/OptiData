using OptiData.Application.Assistant.Commands.AskAssistant;

namespace OptiData.Presentation.Models
{
    public class AssistantViewModel
    {
        public AskAssistantCommand Command { get; set; }
        public string Answer { get; set; }
        public string ErrorMessage { get; set; }

        public AssistantViewModel()
        {
            Command = new AskAssistantCommand();
            Answer = string.Empty;
            ErrorMessage = string.Empty;
        }
    }
}
