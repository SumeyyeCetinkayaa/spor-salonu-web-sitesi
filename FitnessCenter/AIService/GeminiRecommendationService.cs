using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FitnessCenter.AIService
{
    public class GeminiRecommendationService : IAIRecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiRecommendationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public async Task<string> GetRecommendationAsync(string prompt)
        {
            // SENİN LİSTENDEKİ EN GÜNCEL MODEL: 'gemini-flash-latest'
            // Bu model senin hesabında tanımlı, listede gördük.
            var endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{endpoint}?key={_apiKey}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<GeminiResponse>(resultJson);
                    return result?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "Cevap boş döndü.";
                }
                else
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    return $"HATA: {response.StatusCode}. Detay: {errorDetail}";
                }
            }
            catch (Exception ex)
            {
                return "Bağlantı hatası: " + ex.Message;
            }
        }
    }

    // JSON Sınıfları
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; }
    }
    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }
    }
    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; }
    }
    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}