using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FitnessCenter.Models
{
    public class TrainerAvailability
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }

        [ValidateNever]   // Formdan dolmuyor, EF dolduracak
        public Trainer? Trainer { get; set; }

        // System.DayOfWeek enum'unun integer karşılığı
        // Sunday = 0, Monday = 1, ... Saturday = 6
        [Range(0, 6)]
        public int DayOfWeek { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [NotMapped]
        public string DayName => Enum.GetName(typeof(DayOfWeek), DayOfWeek) ?? "";


    }
}
