using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FitnessCenter.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int MemberId { get; set; }

        [ValidateNever]
        public Member? Member { get; set; }

        public int TrainerId { get; set; }

        [ValidateNever]
        public Trainer? Trainer { get; set; }

        public int ServiceId { get; set; }

        [ValidateNever]
        public Service? Service { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime StartTime { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime EndTime { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // ÖNEMLİ: Artık string
        [ScaffoldColumn(false)]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
    }

    public static class AppointmentStatus
    {
        public const string Pending = "Beklemede";
        public const string Approved = "Onaylandı";
        public const string Rejected = "Reddedildi";
        public const string Cancelled = "İptal";
    }
}
