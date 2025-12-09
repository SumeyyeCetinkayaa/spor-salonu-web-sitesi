using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class AIRecommendationViewModel
    {
        [Required(ErrorMessage = "Boy alanı zorunludur.")]
        [Range(100, 250, ErrorMessage = "Boy 100 ile 250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public int HeightCm { get; set; }

        [Required(ErrorMessage = "Kilo alanı zorunludur.")]
        [Range(30, 250, ErrorMessage = "Kilo 30 ile 250 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        public int WeightKg { get; set; }

        [Required(ErrorMessage = "Yaş alanı zorunludur.")]
        [Range(12, 100, ErrorMessage = "Yaş 12 ile 100 arasında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Hedef seçmelisiniz.")]
        [Display(Name = "Hedef")]
        public string Goal { get; set; }

        // Yapay zekâ cevabını göstermek için
        [Display(Name = "Yapay Zeka Önerisi")]
        public string? RecommendationText { get; set; }
    }
}
