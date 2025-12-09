using System;
using System.Threading.Tasks;
using FitnessCenter.Models;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace FitnessCenter.AIService
{
    public class OpenAIRecommendationService : IAIRecommendationService
    {
        private readonly ChatClient _chatClient;

        public OpenAIRecommendationService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenAI:ApiKey appsettings.json içinde bulunamadı.");

            _chatClient = new ChatClient(
                model: "gpt-4o-mini",
                apiKey: apiKey
            );
        }

        public async Task<string> GetRecommendationAsync(AIRecommendationViewModel model)
        {
            var prompt = $"""
Sen bir spor salonu antrenörü ve diyet uzmanısın.

Kullanıcının bilgileri:
- Boy: {model.HeightCm} cm
- Kilo: {model.WeightKg} kg
- Yaş: {model.Age}
- Hedef: {model.Goal}

Türkçe olarak:
1) Kısa değerlendirme
2) 4–5 maddelik egzersiz programı
3) 1 günlük örnek beslenme planı
Net ve anlaşılır bir metin üret.
""";

            try
            {
                ChatCompletion completion = await _chatClient.CompleteChatAsync(prompt);
                return completion.Content[0].Text;
            }
            catch (Exception)
            {
                // Burada loglama yapmak istersen ex parametresini kullanabilirsin.
                // Ödeve uygun, kullanıcıya görünen mesaj:
                return "Şu anda yapay zekâ servisine bağlanılamıyor (API kotası dolu veya servis geçici olarak kullanılamıyor). " +
                       "Lütfen daha sonra tekrar deneyiniz.";
            }
        }
    }
}
