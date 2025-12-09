using System.Threading.Tasks;
using FitnessCenter.Models;

namespace FitnessCenter.AIService
{
    public interface IAIRecommendationService
    {
        Task<string> GetRecommendationAsync(AIRecommendationViewModel model);
    }
}
