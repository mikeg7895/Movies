using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]      
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Genre { get; set; }
        public int Year { get; set; }
        public string? Description { get; set; }

        public int UserId { get; set; }
    }
}
