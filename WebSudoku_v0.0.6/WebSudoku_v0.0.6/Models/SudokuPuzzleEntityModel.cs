using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSudoku_v0._0._6.Models
{
    public class SudokuPuzzleEntityModel : EntityBase, ISudokuPuzzleEntityModel
    {
        [Required]
        [StringLength(81, MinimumLength = 81, ErrorMessage = "Values must be exactly 81 characters long.")]
        [RegularExpression(@"^[1-9\.]{81}$", ErrorMessage = "Values must contain only digits 1-9 and ' ' for empty cells.")]
        [Column("Values")]
        public string Values { get; set; } = string.Empty;

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Difficulty")]
        [MaxLength(10, ErrorMessage = "Difficulty must be 10 characters or less.")]
        public string Difficulty { get; set; } = string.Empty;

        public SudokuPuzzleEntityModel(string values, string difficulty, Guid id)
        {
            Values = values;
            Difficulty = difficulty;
            Id = id;
        }
    }
}
