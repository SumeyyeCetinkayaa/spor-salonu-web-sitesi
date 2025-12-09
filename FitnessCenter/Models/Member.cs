using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessCenter.Models
{
    public class Member
    {
        public int Id { get; set; }  // Primary Key

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string? Email { get; set; }
        public string? Phone { get; set; }

        // Sadece tarih
        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        public string? Goal { get; set; }

        // Formda gösterme, sadece sunucu tarafından set edilsin
        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    }
}
