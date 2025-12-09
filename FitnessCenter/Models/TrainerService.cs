namespace FitnessCenter.Models
{
    public class TrainerService
    {
        public int Id { get; set; }    // Basitlik için tekil PK

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
