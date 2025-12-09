using System.Threading.Tasks;
using FitnessCenter.Models;
using FitnessCenter.AIService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IAIRecommendationService _aiService;

        public AIController(IAIRecommendationService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AIRecommendationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AIRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.RecommendationText = await _aiService.GetRecommendationAsync(model);

            return View(model);
        }
    }
}
