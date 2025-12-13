using System.Threading.Tasks;
using FitnessCenter.Models;

namespace FitnessCenter.AIService // Proje adın farklıysa namespace'i düzelt
{
    public interface IAIRecommendationService
    {
        Task<string> GetRecommendationAsync(string prompt);
    }
}