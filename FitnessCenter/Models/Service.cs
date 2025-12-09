namespace FitnessCenter.Models
{
    public class Service
    {
        public int Id { get; set; }  // PK

        public string Name { get; set; } = null!;          // Örn: "Fitness", "Yoga"
        public string? Description { get; set; }

        public int DurationMinutes { get; set; }           // Örn: 60
        public decimal Price { get; set; }                 // Örn: 250.00

        public bool IsActive { get; set; } = true;

        // İlişkiler
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
