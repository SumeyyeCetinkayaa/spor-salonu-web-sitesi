using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using FitnessCenter.AIService;
using System.Threading.Tasks;

namespace FitnessCenter.Controllers
{
    public class AIController : Controller
    {
        private readonly IAIRecommendationService _aiService;

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
            // Model validasyonu (Basit kontrol)
            if (model.HeightCm == 0 || model.WeightKg == 0)
            {
                // Sayfa ilk yüklendiğinde veya boş veri gelirse hata vermesin
                return View(model ?? new AIRecommendationViewModel());
            }

            try
            {
                // --- GELİŞMİŞ PROMPT MÜHENDİSLİĞİ ---
                // Yapay zekaya HTML formatında cevap vermesini söylüyoruz.
                // Böylece View tarafında @Html.Raw() ile şık bir görüntü elde edeceğiz.

                var prompt = $@"
                    Sen dünyanın en iyi, en motive edici spor ve beslenme koçusun.
                    Aşağıdaki özelliklere sahip bir üye için %100 kişiselleştirilmiş bir program hazırla.

                    ÜYE PROFİLİ:
                    - Cinsiyet: {model.Gender}
                    - Yaş: {model.Age}
                    - Boy: {model.HeightCm} cm
                    - Kilo: {model.WeightKg} kg
                    - Aktivite Seviyesi: {model.ActivityLevel}
                    - Ana Hedef: {model.Goal}
                    - Sağlık Sorunu/Sakatlık: {(string.IsNullOrEmpty(model.HealthIssues) ? "Yok" : model.HealthIssues)}

                    GÖREVİN:
                    Aşağıdaki formatta detaylı bir rehber hazırla. 
                    Cevabı saf HTML formatında ver (div, h3, ul, li, strong etiketlerini kullan).
                    Asla ```html veya ``` gibi kod blokları kullanma, sadece direkt HTML içeriği ver.

                    İSTENEN İÇERİK YAPISI:
                    
                    <h3 class='text-danger'>1. GENEL ANALİZ VE MOTİVASYON</h3>
                    <p>Burada Vücut Kitle İndeksini (BMI) hesapla, yorumla ve üyeye motive edici, ismine özel hissettiren bir giriş yap.</p>

                    <h3 class='text-danger'>2. BESLENME PLANI</h3>
                    <p>Günlük alması gereken tahmini kalori ve makro (protein/karbonhidrat/yağ) oranlarını belirt.</p>
                    <ul>
                        <li><strong>Kahvaltı:</strong> Örnek menü</li>
                        <li><strong>Öğle:</strong> Örnek menü</li>
                        <li><strong>Akşam:</strong> Örnek menü</li>
                        <li><strong>Ara Öğün:</strong> Örnek seçenekler</li>
                    </ul>

                    <h3 class='text-danger'>3. HAFTALIK ANTRENMAN PROGRAMI</h3>
                    <p>Hedefine ({model.Goal}) uygun olarak haftalık programı gün gün listele.</p>
                    <ul>
                        <li><strong>Pazartesi:</strong> ...</li>
                        <li><strong>Çarşamba:</strong> ...</li>
                        <li><strong>Cuma:</strong> ...</li>
                        <li>(Diğer günler dinlenme veya aktif dinlenme)</li>
                    </ul>

                    <h3 class='text-danger'>4. ALTIN TAVSİYELER</h3>
                    <ul>
                        <li>Su tüketimi önerisi</li>
                        <li>Uyku düzeni önerisi</li>
                        <li>Varsa sağlık sorununa ({model.HealthIssues}) özel uyarı.</li>
                    </ul>
                ";

                // Servisi çağır
                var suggestion = await _aiService.GetRecommendationAsync(prompt);

                // Cevabı View'e taşı
                // View tarafında @Html.Raw(ViewBag.Recommendation) kullandığımız için buraya atıyoruz.
                ViewBag.Recommendation = suggestion;

                // Yedek olarak modele de atalım
                model.RecommendationText = suggestion;
            }
            catch (System.Exception ex)
            {
                ViewBag.Recommendation = $"<div class='alert alert-danger'>Üzgünüz, yapay zeka servisine bağlanırken bir hata oluştu: {ex.Message}</div>";
            }

            return View(model);
        }
    }
}