using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using FitnessCenter.AIService; // Interface'i kullanmak için bu gerekli

namespace FitnessCenter.Controllers
{
    public class AIController : Controller
    {
        // Artık HuggingFace key falan yok, sadece servisimizi çağırıyoruz
        private readonly IAIRecommendationService _aiService;

        // Constructor: Program.cs'te tanımladığımız servisi buraya alıyoruz
        public AIController(IAIRecommendationService aiService)
        {
            _aiService = aiService;
        }

        // GET: /AI/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(new AIRecommendationViewModel());
        }

        // POST: /AI/Index
        [HttpPost]
        public async Task<IActionResult> Index(AIRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Model boşsa hata vermesin diye boş model dönüyoruz
                return View(model ?? new AIRecommendationViewModel());
            }

            try
            {
                // 1. Senin yazdığın o harika promptu (istemi) hazırlıyoruz
                var prompt = $@"Sen deneyimli bir spor bilimci ve diyetisyensin. 
Kullanıcının verileri:
- Boy: {model.HeightCm} cm
- Kilo: {model.WeightKg} kg
- Yaş: {model.Age}
- Hedef: {model.Goal}

Görevlerin:
1. Kısa, motive edici bir giriş paragrafı yaz (en fazla 3 cümle).
2. Vücut kitle indeksini (BMI) hesaplayıp sınıflandır.
3. Aşağıdaki başlıklar altında madde madde öneri ver:
   **1. Hedef Özeti**
   **2. Beslenme Planı** (Kalori ve makro önerileriyle)
   **3. Egzersiz Planı (Haftalık Program)**
   **4. Günlük Alışkanlıklar**
   **5. Uyarılar**

Kurallar:
- Tamamen Türkçe yaz.
- Üslup: Profesyonel, net, destekleyici ve ciddi; emoji kullanma.
- ÖNEMLİ: Matematiksel formülleri yazarken asla LaTeX formatı veya '$' işareti kullanma. 
    Hesaplamaları düz metin olarak yaz. (Örn: '66 / 1.61 karesi' gibi değil, sadece sonucu söyle).";
                // 2. Servisi çağırıp cevabı alıyoruz (HuggingFace yerine Gemini'ye gidiyor)
                var suggestion = await _aiService.GetRecommendationAsync(prompt);

                // 3. Cevabı ekrana gönderiyoruz
                model.RecommendationText = suggestion;
                ViewBag.AIResponse = suggestion;
            }
            catch (Exception ex)
            {
                var hataMesaji = "AI servisine bağlanırken hata oluştu: " + ex.Message;
                model.RecommendationText = hataMesaji;
                ViewBag.AIResponse = hataMesaji;
            }

            return View(model);
        }
    }
}