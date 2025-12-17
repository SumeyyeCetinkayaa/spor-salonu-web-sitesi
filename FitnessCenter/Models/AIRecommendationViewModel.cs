using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class AIRecommendationViewModel
    {
        // --- 1. MEVCUT GÜVENLİ ALANLARIN (Koruyoruz) ---

        [Required(ErrorMessage = "Yaş alanı zorunludur.")]
        [Range(12, 100, ErrorMessage = "Yaş 12 ile 100 arasında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Boy alanı zorunludur.")]
        [Range(100, 250, ErrorMessage = "Boy 100 ile 250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public int HeightCm { get; set; }

        [Required(ErrorMessage = "Kilo alanı zorunludur.")]
        [Range(30, 250, ErrorMessage = "Kilo 30 ile 250 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        public int WeightKg { get; set; }

        [Required(ErrorMessage = "Hedef seçmelisiniz.")]
        [Display(Name = "Hedef")]
        public string Goal { get; set; }

        // --- 2. YENİ EKLEDİĞİMİZ ALANLAR (AI daha iyi çalışsın diye) ---

        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = "Belirtilmemiş"; // Varsayılan değer

        [Display(Name = "Aktivite Seviyesi")]
        public string ActivityLevel { get; set; } = "Orta";

        [Display(Name = "Sağlık Sorunu / Sakatlık")]
        public string? HealthIssues { get; set; } // Soru işareti (?) var, yani boş bırakılabilir.

        // --- 3. CEVAP ALANI ---

        [Display(Name = "Yapay Zeka Önerisi")]
        public string? RecommendationText { get; set; }
    }
}