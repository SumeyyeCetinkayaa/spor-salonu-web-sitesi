using System.Net.Http.Headers;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;


namespace FitnessCenter.Controllers
{
    public class AIController : Controller
    {
        private readonly string _hfApiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public AIController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _hfApiKey = configuration["HuggingFace:ApiKey"];
            _httpClientFactory = httpClientFactory;
        }

        // GET: /AI/Index
        public IActionResult Index()
        {
            return View(new AIRecommendationViewModel());
        }

        // POST: /AI/Index
        [HttpPost]
        public async Task<IActionResult> Index(AIRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var suggestion = await GetSuggestionFromHuggingFace(model);

                // 🔹 ViewModel’deki alanın adı RecommendationText
                model.RecommendationText = suggestion;
            }
            catch (Exception ex)
            {
                model.RecommendationText = "AI servisine bağlanılamıyor. " + ex.Message;
            }

            return View(model);
        }

        private async Task<string> GetSuggestionFromHuggingFace(AIRecommendationViewModel model)
        {
            var client = _httpClientFactory.CreateClient();

            // 🔹 HuggingFace Token
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _hfApiKey);

            var inputText =
    $@"Sen deneyimli bir spor bilimci ve diyetisyensin. 
Kullanıcının verileri:
- Boy: {model.HeightCm} cm
- Kilo: {model.WeightKg} kg
- Yaş: {model.Age}
- Hedef: {model.Goal}

Görevlerin:
1. Kısa, motive edici bir giriş paragrafı yaz (en fazla 3 cümle).
2. Vücut kitle indeksini (BMI) hesaplayıp sınıflandır (örnek: normal, fazla kilolu vb.).
3. Aşağıdaki başlıklar altında madde madde öneri ver:
   **1. Hedef Özeti**
   **2. Beslenme Planı**
   **3. Egzersiz Planı (Haftalık Program)**
   **4. Günlük Alışkanlıklar**
   **5. Uyarılar / Dikkat Edilmesi Gerekenler**

Kurallar:
- Tamamen Türkçe yaz.
- Sayısal öneriler ver (örnek: günde yaklaşık 1600–1800 kcal, haftada 4 gün 30 dk yürüyüş gibi).
- Kullanıcının başlangıç seviyesi olduğu varsay, hareketleri çok zorlaştırma.
- Tıbbi tanı koyma ve ilaç önermeden kaçın; gerekirse 'doktoruna danış' uyarısı ekle.
- Üslup: Profesyonel, net, destekleyici ve ciddi; emoji kullanma.";


            // 🔹 OpenAI uyumlu chat/completions gövdesi
            var payload = new
            {
                model = "moonshotai/Kimi-K2-Instruct-0905",   // Inference Providers dokümanındaki örnek model :contentReference[oaicite:2]{index=2}
                messages = new[]
                {
            new { role = "user", content = inputText }
        },
                max_tokens = 512
            };

            var response = await client.PostAsJsonAsync(
                "https://router.huggingface.co/v1/chat/completions",
                payload
            );

            var json = await response.Content.ReadAsStringAsync();

            // 🔹 HTTP hata kodu geldiyse JSON parse etmeye çalışma
            if (!response.IsSuccessStatusCode)
            {
                return $"HuggingFace hata döndürdü ({(int)response.StatusCode} - {response.ReasonPhrase}): {json}";
            }

            try
            {
                var obj = JObject.Parse(json);
                var generated = (string?)obj["choices"]?[0]?["message"]?["content"];

                if (string.IsNullOrWhiteSpace(generated))
                    return "Bir sonuç üretilemedi.";

                // Birden fazla boş satırı 2 satıra indir
                generated = Regex.Replace(generated, @"\n{3,}", "\n\n");

                // Baştaki/sondaki boşlukları temizle
                generated = generated.Trim();

                return generated;

            }
            catch
            {
                return $"Modelden beklenmeyen cevap alındı: {json}";
            }
        }


    }
}
