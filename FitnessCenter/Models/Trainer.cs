using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenter.Models
{
    public class Trainer
    {
        public int Id { get; set; }  // PK

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Uzmanlık alanı: örn. "Kilo verme, kas geliştirme"
        public string? Specialization { get; set; }

        public string? Bio { get; set; }

        public bool IsActive { get; set; } = true;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        // İlişkiler
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
