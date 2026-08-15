using System.Threading.Tasks;

namespace OptiData.Application.Interfaces
{
    public interface IAssistantService
    {
        Task<string> AskQuestionAsync(string userQuestion);
    }
}
